const CART_KEY = "shopsphere_cart";
const CURRENT_USER_KEY = "shopsphere_current_user";
const ENABLE_DEMO_CART = true;

const demoCartItems = [
  {
    id: 101,
    title: "Nova Bluetooth Kulaklık Gürültü Engelleme Özellikli",
    seller: "novastore",
    price: 589,
    quantity: 1,
    selected: true,
    visualClass: "visual-1"
  },
  {
    id: 102,
    title: "Urban Kadın Sneaker Spor Ayakkabı Beyaz",
    seller: "gumussporayakkabi",
    price: 4967.99,
    quantity: 1,
    selected: true,
    visualClass: "visual-4"
  }
];

const cartItemsContainer = document.getElementById("cartItems");
const emptyCart = document.getElementById("emptyCart");
const cartItemCount = document.getElementById("cartItemCount");
const selectAllInput = document.getElementById("selectAll");
const deleteSelectedButton = document.getElementById("deleteSelectedButton");

const summaryProductLabel = document.getElementById("summaryProductLabel");
const summarySubtotal = document.getElementById("summarySubtotal");
const summaryShipping = document.getElementById("summaryShipping");
const summaryDiscount = document.getElementById("summaryDiscount");
const summaryTotal = document.getElementById("summaryTotal");
const checkoutButton = document.getElementById("checkoutButton");
const checkoutMessage = document.getElementById("checkoutMessage");

function getCart() {
  const rawCart = localStorage.getItem(CART_KEY);

  if (!rawCart && ENABLE_DEMO_CART) {
    saveCart(demoCartItems);
    return demoCartItems;
  }

  try {
    return JSON.parse(rawCart || "[]");
  } catch {
    return [];
  }
}

function saveCart(cart) {
  localStorage.setItem(CART_KEY, JSON.stringify(cart));
}

function getCurrentUser() {
  try {
    return JSON.parse(localStorage.getItem(CURRENT_USER_KEY) || "null");
  } catch {
    return null;
  }
}

function formatPrice(value) {
  return value.toLocaleString("tr-TR", {
    minimumFractionDigits: value % 1 === 0 ? 0 : 2,
    maximumFractionDigits: 2
  }) + " TL";
}

function escapeHTML(value) {
  return String(value)
    .replaceAll("&", "&amp;")
    .replaceAll("<", "&lt;")
    .replaceAll(">", "&gt;")
    .replaceAll('"', "&quot;")
    .replaceAll("'", "&#039;");
}

function groupCartBySeller(cart) {
  return cart.reduce((groups, item) => {
    const sellerName = item.seller || "ShopSphere";

    if (!groups[sellerName]) {
      groups[sellerName] = [];
    }

    groups[sellerName].push(item);
    return groups;
  }, {});
}

function calculateSummary(cart) {
  const selectedItems = cart.filter((item) => item.selected);
  const selectedQuantity = selectedItems.reduce((total, item) => total + item.quantity, 0);

  const subtotal = selectedItems.reduce((total, item) => {
    return total + item.price * item.quantity;
  }, 0);

  const shipping = selectedItems.length > 0 ? 49.9 : 0;
  const discount = selectedItems.length > 0 ? Math.round(subtotal * 0.03 * 100) / 100 : 0;
  const total = Math.max(subtotal + shipping - discount, 0);

  return {
    selectedQuantity,
    subtotal,
    shipping,
    discount,
    total
  };
}

function renderCart() {
  const cart = getCart();

  cartItemCount.textContent = `(${cart.length})`;

  if (cart.length === 0) {
    cartItemsContainer.innerHTML = "";
    emptyCart.classList.remove("hidden");
    selectAllInput.checked = false;
    renderSummary(cart);
    return;
  }

  emptyCart.classList.add("hidden");

  const groupedCart = groupCartBySeller(cart);

  cartItemsContainer.innerHTML = Object.entries(groupedCart)
    .map(([seller, items]) => {
      const itemHTML = items
        .map((item) => {
          return `
            <article class="cart-item" data-id="${item.id}">
              <input 
                class="item-check" 
                type="checkbox" 
                ${item.selected ? "checked" : ""}
                aria-label="Ürünü seç"
              />

              <div class="item-thumb">
                <div class="item-visual ${item.visualClass || "visual-0"}"></div>
              </div>

              <div class="item-info">
                <div class="item-campaign">Sepette indirimli ürün</div>
                <h2 class="item-title">${escapeHTML(item.title)}</h2>
                <p class="item-seller">Satıcı: ${escapeHTML(item.seller || "ShopSphere")}</p>
              </div>

              <div class="item-shipping">
                Ara Kargo: <strong>Ücretsiz Kargo</strong>
              </div>

              <div class="item-price-area">
                <strong class="item-price">${formatPrice(item.price * item.quantity)}</strong>

                <div class="quantity-control" aria-label="Adet kontrolü">
                  <button class="decrease-button" type="button">−</button>
                  <span>${item.quantity}</span>
                  <button class="increase-button" type="button">+</button>
                </div>

                <button class="remove-item" type="button">Sil</button>
              </div>
            </article>
          `;
        })
        .join("");

      return `
        <section class="cart-store-group">
          <div class="store-header">
            <strong>${escapeHTML(seller)}</strong>
            <span>9.9</span>
          </div>

          ${itemHTML}
        </section>
      `;
    })
    .join("");

  selectAllInput.checked = cart.every((item) => item.selected);
  renderSummary(cart);
}

function renderSummary(cart) {
  const summary = calculateSummary(cart);

  summaryProductLabel.textContent = `Ürün Tutarı (${summary.selectedQuantity} Adet)`;
  summarySubtotal.textContent = formatPrice(summary.subtotal);
  summaryShipping.textContent = formatPrice(summary.shipping);
  summaryDiscount.textContent = `-${formatPrice(summary.discount)}`;
  summaryTotal.textContent = formatPrice(summary.total);
}

function updateCartItem(id, updater) {
  const cart = getCart();

  const nextCart = cart.map((item) => {
    if (String(item.id) !== String(id)) {
      return item;
    }

    return updater(item);
  });

  saveCart(nextCart);
  renderCart();
}

function removeCartItem(id) {
  const cart = getCart();

  const nextCart = cart.filter((item) => {
    return String(item.id) !== String(id);
  });

  saveCart(nextCart);
  renderCart();
}

cartItemsContainer.addEventListener("click", (event) => {
  const cartItem = event.target.closest(".cart-item");

  if (!cartItem) return;

  const itemId = cartItem.dataset.id;

  if (event.target.classList.contains("increase-button")) {
    updateCartItem(itemId, (item) => ({
      ...item,
      quantity: item.quantity + 1
    }));
  }

  if (event.target.classList.contains("decrease-button")) {
    updateCartItem(itemId, (item) => ({
      ...item,
      quantity: Math.max(item.quantity - 1, 1)
    }));
  }

  if (event.target.classList.contains("remove-item")) {
    removeCartItem(itemId);
  }
});

cartItemsContainer.addEventListener("change", (event) => {
  if (!event.target.classList.contains("item-check")) return;

  const cartItem = event.target.closest(".cart-item");
  const itemId = cartItem.dataset.id;

  updateCartItem(itemId, (item) => ({
    ...item,
    selected: event.target.checked
  }));
});

selectAllInput.addEventListener("change", () => {
  const cart = getCart();

  const nextCart = cart.map((item) => ({
    ...item,
    selected: selectAllInput.checked
  }));

  saveCart(nextCart);
  renderCart();
});

deleteSelectedButton.addEventListener("click", () => {
  const cart = getCart();

  const nextCart = cart.filter((item) => {
    return !item.selected;
  });

  saveCart(nextCart);
  renderCart();
});

checkoutButton.addEventListener("click", () => {
  const cart = getCart();
  const selectedItems = cart.filter((item) => item.selected);

  checkoutMessage.textContent = "";

  if (selectedItems.length === 0) {
    checkoutMessage.textContent = "Devam etmek için en az bir ürün seçmelisin.";
    return;
  }

  const currentUser = getCurrentUser();

  if (!currentUser) {
    window.location.href = "login.html?redirect=cart.html";
    return;
  }

  const selectedAddress =
    typeof getSelectedAddress === "function" ? getSelectedAddress() : null;

  if (!selectedAddress) {
    checkoutMessage.textContent = "Devam etmek için teslimat adresi seçmelisin.";

    if (typeof openAddressModal === "function") {
      openAddressModal();
    }

    return;
  }

  window.location.href = "checkout.html";
});

renderCart();