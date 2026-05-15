const PRODUCTS_PER_PAGE = 40;

let currentPage = 1;

const categoryMap = {
  "moda": "Moda",
  "elektronik": "Elektronik",
  "ev-yasam": "Ev & Yaşam",
  "anne-bebek": "Anne & Bebek",
  "kozmetik": "Kozmetik & Kişisel Bakım",
  "mucevher-saat": "Mücevher & Saat",
  "spor-outdoor": "Spor & Outdoor",
  "kitap-muzik-film-oyun": "Kitap, Müzik, Film, Oyun",
  "otomotiv": "Otomotiv & Motosiklet"
};

const categoryProductNames = {
  "moda": [
    "Oversize Basic Tişört",
    "Kadın Sneaker Günlük Ayakkabı",
    "Erkek Slim Fit Gömlek",
    "Deri Görünümlü Ceket"
  ],
  "elektronik": [
    "Kablosuz Kulaklık Bluetooth 5.3",
    "Akıllı Saat Spor Takip Özellikli",
    "Tablet 10.1 İnç 128 GB Wi-Fi",
    "Gaming Mouse RGB Aydınlatmalı"
  ],
  "ev-yasam": [
    "Modern Masa Lambası",
    "Çift Kişilik Nevresim Takımı",
    "Seramik Kahve Fincanı Seti",
    "Akıllı LED Ampul"
  ],
  "anne-bebek": [
    "Bebek Bakım Çantası",
    "Pamuklu Bebek Battaniyesi",
    "Biberon Seti",
    "Oyuncak Aktivite Halısı"
  ],
  "kozmetik": [
    "Mat Ruj Seti",
    "Cilt Bakım Serumu",
    "Nemlendirici Krem",
    "Makyaj Fırça Seti"
  ],
  "mucevher-saat": [
    "Minimal Kol Saati",
    "Gümüş Kolye",
    "Çelik Bileklik",
    "Zirkon Taşlı Yüzük"
  ],
  "spor-outdoor": [
    "Yoga Matı",
    "Koşu Ayakkabısı",
    "Outdoor Sırt Çantası",
    "Termos Matara"
  ],
  "kitap-muzik-film-oyun": [
    "Roman Seti",
    "Bluetooth Hoparlör",
    "Strateji Masa Oyunu",
    "Film Koleksiyon Paketi"
  ],
  "otomotiv": [
    "Araç Telefon Tutucu",
    "Oto Koltuk Kılıfı",
    "Lastik Parlatıcı",
    "Motosiklet Eldiveni"
  ]
};

const brands = [
  "ShopTech",
  "Nova",
  "PixelPro",
  "SoundMax",
  "Urban",
  "Prime",
  "Luna",
  "Atlas"
];

const urlParams = new URLSearchParams(window.location.search);
const activeCategoryKey = urlParams.get("category") || "elektronik";
const activeCategoryName = categoryMap[activeCategoryKey] || "Elektronik";

const productNamePool =
  categoryProductNames[activeCategoryKey] || categoryProductNames["elektronik"];

const products = Array.from({ length: 96 }, (_, index) => {
  const productNumber = index + 1;
  const basePrice = 699 + productNumber * 83;

  return {
    id: productNumber,
    title: `${brands[index % brands.length]} ${productNamePool[index % productNamePool.length]}`,
    oldPrice: basePrice + 450,
    price: basePrice,
    rating: (4 + ((index % 10) / 10)).toFixed(1),
    reviewCount: 24 + productNumber * 3,
    badge: index % 3 === 0 ? "HIZLI KARGODA" : "n11'in TEKLİFİ"
  };
});

const productGrid = document.getElementById("productGrid");
const pageInfo = document.getElementById("pageInfo");
const prevPageButton = document.getElementById("prevPage");
const nextPageButton = document.getElementById("nextPage");

const categoryTitle = document.getElementById("categoryTitle");
const categoryResultName = document.getElementById("categoryResultName");
const sidebarCategoryName = document.getElementById("sidebarCategoryName");
const breadcrumb = document.getElementById("breadcrumb");
const searchInput = document.getElementById("searchInput");

function initializeCategoryUI() {
  document.title = `ShopSphere | ${activeCategoryName}`;

  categoryTitle.textContent = activeCategoryName;
  categoryResultName.textContent = activeCategoryName;
  sidebarCategoryName.textContent = activeCategoryName;
  breadcrumb.textContent = `Ana Sayfa / ${activeCategoryName}`;
  searchInput.placeholder = `${activeCategoryName} ürünlerinde ara`;

  const categoryLinks = document.querySelectorAll(".category-item");

  categoryLinks.forEach((link) => {
    if (link.dataset.category === activeCategoryKey) {
      link.classList.add("active");
    } else {
      link.classList.remove("active");
    }
  });
}

function formatPrice(value) {
  return value.toLocaleString("tr-TR");
}

function renderProducts() {
  const startIndex = (currentPage - 1) * PRODUCTS_PER_PAGE;
  const endIndex = startIndex + PRODUCTS_PER_PAGE;
  const visibleProducts = products.slice(startIndex, endIndex);

  productGrid.innerHTML = visibleProducts
    .map((product) => {
      return `
        <article class="product-card">
          <div class="product-image-area">
            <span class="product-badge">${product.badge}</span>
            <button class="favorite-button" type="button" aria-label="Favorilere ekle">♡</button>
            <div class="product-visual"></div>
          </div>

          <div class="shipping-strip">ÜCRETSİZ KARGO</div>

          <div class="product-info">
            <h2 class="product-title">${product.title}</h2>

            <div class="rating">
              ★★★★★ <span>(${product.reviewCount})</span>
            </div>

            <p class="old-price">${formatPrice(product.oldPrice)} TL</p>
            <p class="discount-text">SEPETTE</p>

            <div class="product-bottom">
              <strong class="price">${formatPrice(product.price)} TL</strong>
              <button class="add-cart-button" type="button" aria-label="Sepete ekle">+</button>
            </div>
          </div>
        </article>
      `;
    })
    .join("");

  updatePagination();
}

function updatePagination() {
  const totalPages = Math.ceil(products.length / PRODUCTS_PER_PAGE);

  pageInfo.textContent = `${currentPage} / ${totalPages}`;

  prevPageButton.disabled = currentPage === 1;
  nextPageButton.disabled = currentPage === totalPages;
}

prevPageButton.addEventListener("click", () => {
  if (currentPage > 1) {
    currentPage--;
    renderProducts();
    window.scrollTo({ top: 0, behavior: "smooth" });
  }
});

nextPageButton.addEventListener("click", () => {
  const totalPages = Math.ceil(products.length / PRODUCTS_PER_PAGE);

  if (currentPage < totalPages) {
    currentPage++;
    renderProducts();
    window.scrollTo({ top: 0, behavior: "smooth" });
  }
});

initializeCategoryUI();
renderProducts();