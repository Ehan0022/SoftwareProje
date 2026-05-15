# ShopSphere Frontend Mevcut Yapı ve API Entegrasyon Dokümanı

## 1. Amaç

Bu doküman, ShopSphere frontend tarafında şu ana kadar oluşturulan HTML, CSS ve Vanilla JavaScript yapısının nasıl çalıştığını, hangi verilerin şu anda mock/localStorage üzerinden yönetildiğini ve backend API entegrasyonu sırasında hangi dosyalarda hangi değişikliklerin yapılacağını açıklar.

Proje şu anda canlıya çıkmayacak öğrenme amaçlı bir e-ticaret frontend prototipi olarak tasarlanmıştır. Ancak yapı, backend geldiğinde gerçek API isteklerine dönüştürülebilecek şekilde organize edilmiştir.

---

## 2. Mevcut Frontend Yapısı

Frontend şu anda multi-page Vanilla JavaScript yaklaşımıyla ilerliyor. React, Vue veya Angular gibi bir framework yok. Her ana ekranın kendi HTML, CSS ve JS dosyası var.

### Mevcut sayfalar

```txt
landingPage.html / landingPage.css
categoryPage.html / categoryPage.css / categoryPage.js
login.html
register.html
auth.css / auth.js
cart.html / cart.css / cart.js
checkout.html / checkout.css / checkout.js
addressModal.css / addressModal.js
```

### Genel yaklaşım

```txt
HTML  → sayfanın yapısı
CSS   → layout, responsive davranış, görsel tasarım
JS    → localStorage, DOM render, form validation, sayfa yönlendirme
API   → henüz bağlı değil; bir sonraki aşamada bağlanacak
```

Bu yapı frontend için yeterince modülerdir. Ancak ortak header gibi tekrar eden parçalar şu anda HTML içinde tekrar yazılıyor. Backend/API entegrasyonu için kritik sorun değil; ileride istenirse header template/helper yapısına ayrılabilir.

---

## 3. Şu Anda Kullanılan localStorage Key’leri

Frontend şu anda backend olmadığı için bazı verileri localStorage içinde tutuyor.

```txt
shopsphere_users
shopsphere_current_user
shopsphere_cart
shopsphere_addresses
shopsphere_selected_address_id
```

### 3.1. shopsphere_users

`auth.js` içinde kullanılıyor.

Geçici olarak kayıt olan kullanıcıları tutuyor.

Şu anda örnek veri yapısı:

```js
{
  id: Date.now(),
  firstName: "Ali",
  lastName: "Yılmaz",
  email: "ali@example.com",
  password: "Password1",
  role: "Customer"
}
```

Backend geldiğinde bu tamamen kaldırılacak. Kullanıcı kayıtları localStorage’da tutulmayacak.

### 3.2. shopsphere_current_user

`auth.js`, `cart.js`, `checkout.js` içinde kullanılıyor.

Geçici login state’i tutuyor.

Şu anda örnek veri:

```js
{
  id: 123,
  firstName: "Ali",
  lastName: "Yılmaz",
  email: "ali@example.com",
  role: "Customer"
}
```

Backend geldiğinde cookie-based authentication kullanılacağı için bu yapı kaldırılmalı veya sadece UI cache olarak çok sınırlı kullanılmalıdır. Asıl login durumu `GET /api/auth/me` gibi bir endpoint üzerinden öğrenilmelidir.

### 3.3. shopsphere_cart

`categoryPage.js`, `cart.js`, `checkout.js` içinde kullanılıyor.

Sepete eklenen ürünleri tutuyor.

Şu anda örnek veri:

```js
{
  id: "elektronik-1",
  title: "ShopTech Kablosuz Kulaklık Bluetooth 5.3",
  seller: "ShopTech",
  price: 782,
  quantity: 1,
  selected: true,
  visualClass: "visual-1"
}
```

Backend geldiğinde cart database-backed olacak. Bu nedenle cart verisi API’den alınmalı ve API’ye yazılmalıdır.

### 3.4. shopsphere_addresses

`addressModal.js` içinde kullanılıyor.

Kayıtlı adresleri tutuyor.

Şu anda örnek veri:

```js
{
  id: 1710000000000,
  title: "Ev",
  fullAddress: "Bursa Nilüfer ... açık adres"
}
```

Backend geldiğinde bu veri `Address` tablosuna bağlanmalı.

### 3.5. shopsphere_selected_address_id

`addressModal.js`, `cart.js`, `checkout.js` içinde kullanılıyor.

Seçili adresin id değerini tutuyor.

Backend geldiğinde seçili adres client tarafında tutulabilir; ancak checkout request içinde `addressId` olarak backend’e gönderilmelidir.

---

## 4. Mevcut Sayfaların Mantığı

## 4.1. landingPage.html

Ana sayfadır.

İçerik:

```txt
Header
Search bar
Teslimat adresi butonu
Cart linki
Hesap linki
Kategori barı
Hero/banner alanı
```

Kategori barındaki linkler şu şekilde kategori sayfasına gider:

```txt
categoryPage.html?category=moda
categoryPage.html?category=elektronik
categoryPage.html?category=ev-yasam
categoryPage.html?category=anne-bebek
categoryPage.html?category=kozmetik
categoryPage.html?category=mucevher-saat
categoryPage.html?category=spor-outdoor
categoryPage.html?category=kitap-muzik-film-oyun
categoryPage.html?category=otomotiv
```

Search form submit olunca şuna yönlenir:

```txt
categoryPage.html?category=elektronik&search=<aranan_kelime>
```

### API entegrasyonunda değişecek yerler

Landing page şu anda API’ye ihtiyaç duymadan çalışıyor. İleride istenirse hero banner ve öne çıkan kategoriler API’den çekilebilir.

Opsiyonel endpoint önerileri:

```txt
GET /api/categories
GET /api/home/banners
GET /api/home/featured-products
```

Ancak mevcut proje kapsamında landing page statik kalabilir.

---

## 4.2. categoryPage.html / categoryPage.js

Kategori ürün listeleme sayfasıdır.

İçerik:

```txt
Header
Kategori barı
Sol filtre sidebar
Kategori başlığı
Quick filter chip’leri
Ürün grid’i
Pagination
Cart toast feedback
```

Mevcut JS davranışı:

```txt
URL’den category parametresini okur.
Kategori adına göre mock ürün listesi oluşturur.
Sayfa başına 40 ürün gösterir.
Desktop’ta satır başına 4 ürün gösterir.
Search parametresi varsa ürünleri client-side filtreler.
+ butonuna basınca ürünü shopsphere_cart içine ekler.
Cart icon badge’ini günceller.
Toast gösterir.
```

### Şu anda mock olan kısım

`categoryPage.js` içindeki şu yapılar mock’tur:

```js
categoryMap
categoryProductNames
brands
products = Array.from({ length: 96 }, ...)
```

### API entegrasyonunda değişecek yerler

Kategori ürünleri artık client-side üretilmeyecek. Backend’den alınacak.

Önerilen endpoint:

```txt
GET /api/products
```

Örnek query:

```txt
GET /api/products?category=elektronik&search=laptop&page=1&pageSize=40&sort=smart
```

Filtreler eklendiğinde:

```txt
GET /api/products?category=elektronik&brand=Nova&minPrice=1000&maxPrice=5000&inStock=true&page=1&pageSize=40
```

Beklenen response önerisi:

```json
{
  "items": [
    {
      "id": 1,
      "name": "Kablosuz Kulaklık Bluetooth 5.3",
      "description": "...",
      "price": 1299,
      "oldPrice": 1599,
      "stockQuantity": 12,
      "categoryName": "Elektronik",
      "sellerName": "Nova",
      "imageUrl": null,
      "rating": 4.6,
      "reviewCount": 128
    }
  ],
  "page": 1,
  "pageSize": 40,
  "totalCount": 96,
  "totalPages": 3
}
```

`categoryPage.js` içinde değişecek ana fonksiyonlar:

```txt
products mock array’i kaldırılacak.
renderProducts() API’den gelen items üzerinden render edecek.
getVisibleProducts() kaldırılacak veya sadece local fallback olacak.
updatePagination() backend totalPages değerini kullanacak.
addProductToCart(product) localStorage yerine API’ye POST atacak.
updateCartBadge() sepet toplamını API’den alacak.
```

Sepete ekleme için önerilen endpoint:

```txt
POST /api/cart/items
```

Request body:

```json
{
  "productId": 1,
  "quantity": 1
}
```

Response önerisi:

```json
{
  "success": true,
  "cartItemCount": 3,
  "message": "Ürün sepete eklendi."
}
```

Kritik not:

```txt
Frontend ürün stok kontrolü yapabilir ama asıl stok kontrolü backend’de yapılmalıdır.
```

---

## 4.3. login.html / register.html / auth.js

Login ve register sayfalarıdır.

Mevcut davranış:

```txt
register.html form validation yapar.
Kullanıcıyı shopsphere_users içine kaydeder.
login.html girilen email/password bilgisini shopsphere_users içinde arar.
Başarılı login olursa shopsphere_current_user içine kullanıcı bilgisini yazar.
```

Register alanları:

```txt
Ad
Soyad
E-posta
Şifre
Rol seçimi: Customer / Seller / Admin
Üyelik sözleşmesi checkbox
```

Login alanları:

```txt
E-posta
Şifre
```

### API entegrasyonunda değişecek yerler

`auth.js` içindeki localStorage kullanıcı sistemi kaldırılacak.

Kaldırılacak/geçersiz olacak fonksiyonlar:

```js
getUsers()
saveUsers()
localStorage.setItem("shopsphere_current_user", ...)
```

Backend endpoint önerileri:

```txt
POST /api/auth/register
POST /api/auth/login
POST /api/auth/logout
GET /api/auth/me
```

### Register endpoint

```txt
POST /api/auth/register
```

Request body:

```json
{
  "firstName": "Ali",
  "lastName": "Yılmaz",
  "email": "ali@example.com",
  "password": "Password1",
  "role": "Customer"
}
```

Response önerisi:

```json
{
  "success": true,
  "message": "Registration successful."
}
```

Hata response önerisi:

```json
{
  "success": false,
  "message": "Email already exists.",
  "errors": {
    "email": "Bu e-posta adresi zaten kayıtlı."
  }
}
```

### Login endpoint

```txt
POST /api/auth/login
```

Request body:

```json
{
  "email": "ali@example.com",
  "password": "Password1"
}
```

Response önerisi:

```json
{
  "success": true,
  "user": {
    "id": 1,
    "firstName": "Ali",
    "lastName": "Yılmaz",
    "email": "ali@example.com",
    "role": "Customer"
  }
}
```

Cookie-based auth kullanılacağı için fetch request’lerinde şu ayar gereklidir:

```js
credentials: "include"
```

Örnek:

```js
await fetch("/api/auth/login", {
  method: "POST",
  credentials: "include",
  headers: {
    "Content-Type": "application/json"
  },
  body: JSON.stringify(payload)
});
```

### Me endpoint

```txt
GET /api/auth/me
```

Amaç:

```txt
Sayfa açıldığında kullanıcının login olup olmadığını ve rolünü öğrenmek.
```

Response:

```json
{
  "authenticated": true,
  "user": {
    "id": 1,
    "firstName": "Ali",
    "lastName": "Yılmaz",
    "email": "ali@example.com",
    "role": "Customer"
  }
}
```

Kritik not:

```txt
Şifre asla frontend localStorage’da tutulmamalıdır.
Backend tarafında password hash olarak saklanmalıdır.
```

---

## 4.4. addressModal.css / addressModal.js

Adres seçme ve adres ekleme popup’ıdır.

Yeni sayfaya geçmeden modal olarak açılır.

Mevcut davranış:

```txt
.address-box class’ına sahip header adres butonlarını yakalar.
Sayfaya dinamik olarak address modal HTML’i ekler.
Kayıtlı adresleri localStorage’dan okur.
Yeni açık adres ekler.
Adres başlığı isteğe bağlıdır.
fullAddress zorunludur.
Seçili adres id’sini shopsphere_selected_address_id içinde tutar.
Header’daki adres metnini seçili adrese göre günceller.
```

Adres modeli şu anda:

```js
{
  id: Date.now(),
  title: "Ev",
  fullAddress: "Bursa Nilüfer ..."
}
```

### API entegrasyonunda değişecek yerler

Adresler localStorage yerine backend’den alınacak.

Önerilen endpoint’ler:

```txt
GET /api/addresses
POST /api/addresses
DELETE /api/addresses/{addressId}
```

Opsiyonel:

```txt
PUT /api/addresses/{addressId}
PUT /api/users/me/selected-address
```

### Adres listeleme

```txt
GET /api/addresses
```

Response:

```json
[
  {
    "id": 1,
    "title": "Ev",
    "fullAddress": "Bursa Nilüfer ..."
  }
]
```

### Yeni adres ekleme

```txt
POST /api/addresses
```

Request body:

```json
{
  "title": "Ev",
  "fullAddress": "Bursa Nilüfer ..."
}
```

Response:

```json
{
  "id": 1,
  "title": "Ev",
  "fullAddress": "Bursa Nilüfer ..."
}
```

### Adres silme

```txt
DELETE /api/addresses/{addressId}
```

### Frontend’de değişecek fonksiyonlar

```txt
getAddresses()      → GET /api/addresses
saveAddresses()     → kaldırılacak
saveNewAddress()    → POST /api/addresses
deleteSavedAddress() → DELETE /api/addresses/{id}
getSelectedAddress() → local selected id + API address list veya backend selected address
```

Kritik not:

```txt
Checkout sırasında frontend sadece seçili adresi göstermekle kalmamalı, backend’e addressId göndermelidir.
```

---

## 4.5. cart.html / cart.js

Sepet sayfasıdır.

Mevcut davranış:

```txt
shopsphere_cart içindeki ürünleri okur.
Ürünleri seller’a göre gruplar.
Tümünü seç / tek tek seç çalışır.
Adet artırma / azaltma çalışır.
Ürün silme çalışır.
Seçilenleri silme çalışır.
Sipariş özeti hesaplanır.
Ödemeye Geç butonuna basınca login kontrolü yapılır.
Login yoksa login.html?redirect=cart.html sayfasına gider.
Login var ama adres yoksa address modal açılır.
Login ve adres varsa checkout.html sayfasına gider.
```

### Demo cart notu

`cart.js` içinde şu flag var:

```js
const ENABLE_DEMO_CART = true;
```

Bu flag, sepette veri yokken demo ürünleri otomatik ekler. Backend entegrasyonu öncesinde veya gerçek testlerde bu değer kapatılmalıdır.

```js
const ENABLE_DEMO_CART = false;
```

### API entegrasyonunda değişecek yerler

Cart localStorage’dan değil backend’den alınacak.

Önerilen endpoint’ler:

```txt
GET /api/cart
POST /api/cart/items
PATCH /api/cart/items/{cartItemId}
DELETE /api/cart/items/{cartItemId}
DELETE /api/cart/items/bulk
```

Alternatif olarak productId üzerinden de yapılabilir:

```txt
PATCH /api/cart/items/by-product/{productId}
DELETE /api/cart/items/by-product/{productId}
```

Ancak database-backed CartItem varsa `cartItemId` kullanmak daha temizdir.

### Cart listeleme

```txt
GET /api/cart
```

Response önerisi:

```json
{
  "id": 10,
  "items": [
    {
      "cartItemId": 55,
      "productId": 1,
      "title": "Kablosuz Kulaklık",
      "sellerName": "Nova",
      "price": 1299,
      "quantity": 2,
      "stockQuantity": 12,
      "imageUrl": null
    }
  ],
  "summary": {
    "subtotal": 2598,
    "shipping": 49.9,
    "discount": 77.94,
    "total": 2569.96
  }
}
```

### Adet güncelleme

```txt
PATCH /api/cart/items/{cartItemId}
```

Request body:

```json
{
  "quantity": 3
}
```

### Ürün silme

```txt
DELETE /api/cart/items/{cartItemId}
```

### Seçilenleri silme

```txt
DELETE /api/cart/items/bulk
```

Request body:

```json
{
  "cartItemIds": [55, 56, 57]
}
```

### Frontend’de değişecek fonksiyonlar

```txt
getCart()              → GET /api/cart
saveCart()             → kaldırılacak
updateCartItem()       → PATCH /api/cart/items/{id}
removeCartItem()       → DELETE /api/cart/items/{id}
deleteSelectedButton   → DELETE /api/cart/items/bulk
renderSummary()        → tercihen backend summary kullanacak
checkoutButton click   → auth + address + selected items kontrolünden sonra checkout’a gidecek
```

Kritik not:

```txt
Cart item selected state şu anda frontend localStorage’da tutuluyor. Backend’e bağlanırken bu iki şekilde çözülebilir:
1. selected state sadece frontend’de tutulur, checkout’a selectedCartItemIds gönderilir.
2. backend CartItem üzerinde IsSelected alanı tutulur.
```

Önerilen çözüm:

```txt
selected state frontend’de kalsın.
Checkout request sırasında selectedCartItemIds gönderilsin.
```

---

## 4.6. checkout.html / checkout.js

Ödeme ve sipariş tamamlama ekranıdır.

Mevcut davranış:

```txt
shopsphere_cart içindeki selected=true ürünleri okur.
Seçili adresi gösterir.
Adres yoksa address modal açar.
Login yoksa login.html?redirect=checkout.html sayfasına yönlendirir.
Kart bilgilerini validate eder.
Ödeme gerçek değildir.
Kart formatı doğruysa başarılı popup gösterir.
Kart formatı yanlışsa başarısız popup gösterir.
Popup 4 saniye kalır.
Sonra landingPage.html sayfasına yönlendirir.
Başarılı ödeme sonrası selected ürünleri localStorage sepetinden kaldırır.
```

### Kart format validation

Mevcut kurallar:

```txt
Kart sahibi: Ad Soyad formatında olmalı.
Kart numarası: 16 haneli olmalı ve Luhn algoritmasından geçmeli.
Son kullanma tarihi: AA/YY formatında ve gelecekte olmalı.
CVV: 3 haneli olmalı.
```

Test için geçerli örnek kart:

```txt
4242 4242 4242 4242
12/30
123
```

### API entegrasyonunda değişecek yerler

Checkout artık sadece frontend success üretmemeli. Backend’de order oluşturulmalı, stock validation yapılmalı, cart temizlenmeli.

Önerilen endpoint:

```txt
POST /api/orders/checkout
```

Request body önerisi:

```json
{
  "addressId": 1,
  "cartItemIds": [55, 56],
  "payment": {
    "method": "SimulatedCard",
    "cardHolder": "Ali Yılmaz",
    "cardLast4": "4242"
  }
}
```

Kritik güvenlik notu:

```txt
CVV backend’e gönderilmemeli ve hiçbir yerde saklanmamalıdır.
Tam kart numarası saklanmamalıdır.
Simulated payment için en fazla cardLast4 tutulabilir.
```

Backend response başarılı:

```json
{
  "success": true,
  "orderId": 1001,
  "message": "Order created successfully."
}
```

Backend response başarısız:

```json
{
  "success": false,
  "message": "Insufficient stock for some products.",
  "errors": [
    {
      "productId": 1,
      "reason": "InsufficientStock"
    }
  ]
}
```

### Frontend’de değişecek fonksiyonlar

```txt
getSelectedCartItems()      → GET /api/cart response üzerinden selected item ids
showPaymentResult("success") → sadece API success dönerse çalışacak
removePurchasedItemsFromCart() → kaldırılacak; backend cart temizleyecek
paymentForm submit          → POST /api/orders/checkout atacak
```

Checkout submit yeni mantık:

```txt
1. selected cart item var mı?
2. login var mı? /api/auth/me ile kontrol
3. selected address var mı?
4. kart formatı doğru mu?
5. POST /api/orders/checkout
6. success ise başarılı popup
7. fail ise başarısız popup
8. 4 saniye sonra landingPage.html
```

---

## 5. Ortak API Wrapper Önerisi

Bir sonraki adımda her dosyanın içinde ayrı fetch yazmak yerine ortak `api.js` oluşturulmalıdır.

Dosya:

```txt
api.js
```

Önerilen içerik:

```js
const API_BASE_URL = "https://localhost:5001/api";

async function apiRequest(endpoint, options = {}) {
  const response = await fetch(`${API_BASE_URL}${endpoint}`, {
    credentials: "include",
    headers: {
      "Content-Type": "application/json",
      ...(options.headers || {})
    },
    ...options
  });

  const data = await response.json().catch(() => null);

  if (!response.ok) {
    throw {
      status: response.status,
      message: data?.message || "Bir hata oluştu.",
      errors: data?.errors || null
    };
  }

  return data;
}
```

Her HTML dosyasında ilgili JS dosyasından önce bağlanmalıdır:

```html
<script src="api.js"></script>
<script src="categoryPage.js"></script>
```

---

## 6. Önerilen API Listesi

Aşağıdaki liste frontend’in ihtiyaç duyacağı tüm ana API bağlantılarını kapsar.

## 6.1. Auth API

```txt
POST /api/auth/register
POST /api/auth/login
POST /api/auth/logout
GET  /api/auth/me
```

Kullanılacağı dosyalar:

```txt
auth.js
cart.js
checkout.js
seller.js ileride
admin.js ileride
```

## 6.2. Product API

```txt
GET    /api/products
GET    /api/products/{productId}
POST   /api/seller/products
PUT    /api/seller/products/{productId}
DELETE /api/seller/products/{productId}
DELETE /api/admin/products/{productId}
```

Kullanılacağı dosyalar:

```txt
categoryPage.js
productDetail.js ileride
sellerProducts.js ileride
adminDashboard.js ileride
```

## 6.3. Category API

```txt
GET /api/categories
```

Kullanılacağı dosyalar:

```txt
landingPage.js ileride opsiyonel
categoryPage.js opsiyonel
```

Şu anda kategori listesi statik kalabilir.

## 6.4. Cart API

```txt
GET    /api/cart
POST   /api/cart/items
PATCH  /api/cart/items/{cartItemId}
DELETE /api/cart/items/{cartItemId}
DELETE /api/cart/items/bulk
```

Kullanılacağı dosyalar:

```txt
categoryPage.js
cart.js
checkout.js
```

## 6.5. Address API

```txt
GET    /api/addresses
POST   /api/addresses
PUT    /api/addresses/{addressId}
DELETE /api/addresses/{addressId}
```

Kullanılacağı dosyalar:

```txt
addressModal.js
cart.js
checkout.js
```

## 6.6. Order API

```txt
POST /api/orders/checkout
GET  /api/orders/my
GET  /api/orders/{orderId}
GET  /api/admin/orders
```

Kullanılacağı dosyalar:

```txt
checkout.js
orders.js ileride
adminDashboard.js ileride
```

## 6.7. Admin API

```txt
GET    /api/admin/orders
GET    /api/admin/dashboard
DELETE /api/admin/products/{productId}
```

Kullanılacağı dosyalar:

```txt
adminDashboard.html / adminDashboard.js ileride
```

## 6.8. Seller API

```txt
GET    /api/seller/products
POST   /api/seller/products
PUT    /api/seller/products/{productId}
DELETE /api/seller/products/{productId}
```

Kullanılacağı dosyalar:

```txt
sellerProducts.html / sellerProducts.js ileride
```

---

## 7. API Entegrasyonu Sırasında Dosya Bazlı Değişiklik Planı

## 7.1. Önce api.js oluşturulacak

Amaç:

```txt
Tüm fetch isteklerini tek merkezden yönetmek.
Cookie göndermek.
Hata response’larını standartlaştırmak.
```

## 7.2. auth.js dönüştürülecek

Değişiklik:

```txt
localStorage user sistemi kaldırılacak.
register form → POST /api/auth/register
login form    → POST /api/auth/login
sayfa load    → GET /api/auth/me opsiyonel
```

## 7.3. categoryPage.js dönüştürülecek

Değişiklik:

```txt
mock products kaldırılacak.
GET /api/products ile ürün listelenecek.
POST /api/cart/items ile sepete eklenecek.
GET /api/cart ile badge güncellenecek.
```

## 7.4. cart.js dönüştürülecek

Değişiklik:

```txt
GET /api/cart ile sepet çekilecek.
PATCH /api/cart/items/{id} ile quantity güncellenecek.
DELETE /api/cart/items/{id} ile ürün silinecek.
DELETE /api/cart/items/bulk ile seçili ürünler silinecek.
checkout’a geçmeden önce /api/auth/me ve adres kontrolü yapılacak.
```

## 7.5. addressModal.js dönüştürülecek

Değişiklik:

```txt
GET /api/addresses ile adresler çekilecek.
POST /api/addresses ile adres eklenecek.
DELETE /api/addresses/{id} ile adres silinecek.
```

## 7.6. checkout.js dönüştürülecek

Değişiklik:

```txt
localStorage cart yerine GET /api/cart kullanılacak.
POST /api/orders/checkout ile order oluşturulacak.
Başarı/başarısız popup API response’a göre gösterilecek.
Başarılı sipariş sonrası localStorage’dan cart silinmeyecek; backend cart temizleyecek.
```

---

## 8. Backend’den Beklenen Genel Response Standardı

Frontend’i daha kolay bağlamak için tüm API’lerde benzer response formatı kullanılmalıdır.

Başarılı response:

```json
{
  "success": true,
  "message": "Operation successful.",
  "data": {}
}
```

Hatalı response:

```json
{
  "success": false,
  "message": "Validation failed.",
  "errors": {
    "email": "Email already exists."
  }
}
```

Liste response:

```json
{
  "items": [],
  "page": 1,
  "pageSize": 40,
  "totalCount": 96,
  "totalPages": 3
}
```

---

## 9. Authentication ve Authorization Notları

Frontend rol bazlı UI gösterebilir ama güvenlik frontend’e bırakılmamalıdır.

Örnek:

```txt
Customer seller panelini görmeyebilir.
Ama seller endpoint’ine manuel request atarsa backend engellemelidir.
```

Backend tarafında zorunlu kontroller:

```txt
Customer → cart, checkout, order history
Seller   → kendi ürünlerini ekleme/güncelleme/silme
Admin    → order monitoring, product removal
```

Frontend’de ise sadece UX için yönlendirme yapılır.

Örnek frontend guard:

```js
const me = await apiRequest("/auth/me");

if (!me.authenticated) {
  window.location.href = "login.html";
}
```

---

## 10. Ödeme Simülasyonu Notu

Mevcut checkout ekranı gerçek ödeme almaz.

Frontend şu anda sadece şunları kontrol eder:

```txt
Kart sahibi formatı
16 haneli ve Luhn valid kart numarası
AA/YY formatında geçerli son kullanma tarihi
3 haneli CVV
```

Backend entegrasyonunda da gerçek payment gateway kullanılmayacak. Ancak order oluşturma backend’de yapılmalıdır.

Önerilen yaklaşım:

```txt
Frontend kart formatını kontrol eder.
Backend’e sadece simulated payment bilgisi gönderilir.
Backend stock validation yapar.
Backend order oluşturur.
Backend cart temizler.
Backend success/fail döner.
Frontend popup gösterir.
```

Asla saklanmaması gerekenler:

```txt
Tam kart numarası
CVV
```

Saklanabilecek opsiyonel bilgi:

```txt
cardLast4
paymentStatus
paymentMethod = SimulatedCard
amount
```

---

## 11. Bir Sonraki Geliştirme Sırası

Önerilen sıra:

```txt
1. api.js ortak fetch wrapper oluştur.
2. Backend auth endpoint’leri hazır olunca auth.js’i bağla.
3. Product listing endpoint hazır olunca categoryPage.js mock ürünleri kaldır.
4. Cart endpoint hazır olunca categoryPage.js sepete ekleme ve cart.js sepet yönetimini bağla.
5. Address endpoint hazır olunca addressModal.js’i bağla.
6. Checkout endpoint hazır olunca checkout.js’i bağla.
7. Seller product management sayfasını geliştir.
8. Admin dashboard sayfasını geliştir.
9. Customer order history sayfasını geliştir.
```

Minimum backend entegrasyon sırası:

```txt
Auth → Products → Cart → Address → Checkout/Orders
```

Bu sıra mantıklıdır çünkü checkout çalışması için önce login, ürün, cart ve adres akışının hazır olması gerekir.

---

## 12. Kritik Kontrol Listesi

API entegrasyonuna başlamadan önce şu noktalar netleşmeli:

```txt
Backend base URL ne olacak?
Cookie auth için CORS ayarı yapılacak mı?
Frontend ve backend farklı portta mı çalışacak?
Cart item id mi product id mi kullanılacak?
Selected cart items backend’de mi tutulacak frontend’de mi?
Address selected state backend’de mi tutulacak frontend’de mi?
Checkout request tam olarak hangi body ile order oluşturacak?
Payment simulation backend’de hangi alanları saklayacak?
```

Önerilen kararlar:

```txt
API base URL: https://localhost:5001/api
Auth: Cookie-based, credentials include
Cart update: cartItemId üzerinden
Selected cart items: frontend’de tutulup checkout request ile cartItemIds gönderilsin
Address: backend’de kayıtlı adresler; checkout request içinde addressId gönderilsin
Payment: simulated; CVV ve tam kart numarası saklanmasın
```

---

## 13. Son Durum Özeti

Frontend şu an öğrenme ve demo için yeterli seviyede çalışıyor.

Mevcut özellikler:

```txt
Ana sayfa
Kategori sayfası
Kategoriye göre ürün listeleme
Search parametresi
Pagination
Sepete ürün ekleme
Cart badge ve toast feedback
Login/register mock sistemi
Role seçimi
Cart sayfası
Sepette ürün seçme, adet değiştirme, silme
Adres popup sistemi
Adres ekleme/seçme/silme
Checkout ekranı
Kart format validation
Başarı/başarısız popup
4 saniye sonra ana sayfaya yönlendirme
```

API entegrasyonunda ana hedef:

```txt
localStorage mock verilerini sırasıyla backend endpoint’lerine taşımak.
```

En kritik dosyalar:

```txt
auth.js
categoryPage.js
cart.js
addressModal.js
checkout.js
```

En kritik endpoint’ler:

```txt
POST /api/auth/register
POST /api/auth/login
GET  /api/auth/me
GET  /api/products
POST /api/cart/items
GET  /api/cart
PATCH /api/cart/items/{cartItemId}
DELETE /api/cart/items/{cartItemId}
GET  /api/addresses
POST /api/addresses
POST /api/orders/checkout
```

Bu endpoint’ler hazır olduğunda frontend gerçek backend ile çalışabilecek hale gelir.

