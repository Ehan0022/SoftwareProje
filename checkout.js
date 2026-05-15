const CART_KEY = "shopsphere_cart";
const CURRENT_USER_KEY = "shopsphere_current_user";

const checkoutItems = document.getElementById("checkoutItems");
const emptyCheckout = document.getElementById("emptyCheckout");

const selectedAddressText = document.getElementById("selectedAddressText");
const changeAddressButton = document.getElementById("changeAddressButton");

const summaryProductLabel = document.getElementById("summaryProductLabel");
const summarySubtotal = document.getElementById("summarySubtotal");
const summaryShipping = document.getElementById("summaryShipping");
const summaryDiscount = document.getElementById("summaryDiscount");
const summaryTotal = document.getElementById("summaryTotal");

const paymentForm = document.getElementById("paymentForm");
const cardHolderInput = document.getElementById("cardHolder");
const cardNumberInput = document.getElementById("cardNumber");
const expiryDateInput = document.getElementById("expiryDate");
const cvvInput = document.getElementById("cvv");

const paymentResultOverlay = document.getElementById("paymentResultOverlay");
const paymentResultModal = document.getElementById("paymentResultModal");
const paymentResultIcon = document.getElementById("paymentResultIcon");
const paymentResultTitle = document.getElementById("paymentResultTitle");
const paymentResultText = document.getElementById("paymentResultText");

function getCart() {
  try {
    return JSON.parse(localStorage.getItem(CART_KEY) || "[]");
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

function getSelectedCartItems() {
  return getCart().filter((item) => item.selected);
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

function calculateSummary(items) {
  const selectedQuantity = items.reduce((total, item) => {
    return total + Number(item.quantity || 0);
  }, 0);

  const subtotal = items.reduce((total, item) => {
    return total + Number(item.price || 0) * Number(item.quantity || 0);
  }, 0);

  const shipping = items.length > 0 ? 49.9 : 0;
  const discount = items.length > 0 ? Math.round(subtotal * 0.03 * 100) / 100 : 0;
  const total = Math.max(subtotal + shipping - discount, 0);

  return {
    selectedQuantity,
    subtotal,
    shipping,
    discount,
    total
  };
}

function renderCheckoutItems() {
  const selectedItems = getSelectedCartItems();

  if (selectedItems.length === 0) {
    checkoutItems.innerHTML = "";
    emptyCheckout.classList.remove("hidden");
    renderSummary(selectedItems);
    return;
  }

  emptyCheckout.classList.add("hidden");

  checkoutItems.innerHTML = selectedItems
    .map((item) => {
      return `
        <article class="checkout-item">
          <div class="item-thumb">
            <div class="item-visual ${item.visualClass || "visual-0"}"></div>
          </div>

          <div class="item-info">
            <h3>${escapeHTML(item.title)}</h3>
            <p>Satıcı: ${escapeHTML(item.seller || "ShopSphere")} · Adet: ${item.quantity}</p>
          </div>

          <strong class="item-price">${formatPrice(item.price * item.quantity)}</strong>
        </article>
      `;
    })
    .join("");

  renderSummary(selectedItems);
}

function renderSummary(items) {
  const summary = calculateSummary(items);

  summaryProductLabel.textContent = `Ürün Tutarı (${summary.selectedQuantity} Adet)`;
  summarySubtotal.textContent = formatPrice(summary.subtotal);
  summaryShipping.textContent = formatPrice(summary.shipping);
  summaryDiscount.textContent = `-${formatPrice(summary.discount)}`;
  summaryTotal.textContent = formatPrice(summary.total);
}

function updateSelectedAddressView() {
  const selectedAddress =
    typeof getSelectedAddress === "function" ? getSelectedAddress() : null;

  if (!selectedAddress) {
    selectedAddressText.textContent = "Adres seçilmedi. Devam etmek için teslimat adresi seçmelisin.";
    return;
  }

  selectedAddressText.textContent = selectedAddress.fullAddress;
}

function setFieldError(input, message) {
  const group = input.closest(".form-group");
  const error = group?.querySelector(".error-message");

  if (error) {
    error.textContent = message;
  }

  input.classList.toggle("has-error", Boolean(message));
}

function clearFieldError(input) {
  setFieldError(input, "");
}

function onlyDigits(value) {
  return value.replace(/\D/g, "");
}

function formatCardNumber(value) {
  const digits = onlyDigits(value).slice(0, 16);

  return digits.replace(/(\d{4})(?=\d)/g, "$1 ");
}

function formatExpiryDate(value) {
  const digits = onlyDigits(value).slice(0, 4);

  if (digits.length <= 2) {
    return digits;
  }

  return `${digits.slice(0, 2)}/${digits.slice(2)}`;
}

function isValidCardHolder(value) {
  const normalizedValue = value.trim().replace(/\s+/g, " ");

  return /^[A-Za-zÇĞİÖŞÜçğıöşü\s]+$/.test(normalizedValue)
    && normalizedValue.split(" ").length >= 2
    && normalizedValue.length >= 5;
}

function isValidTurkishCardNumber(value) {
  const digits = onlyDigits(value);

  if (!/^\d{16}$/.test(digits)) {
    return false;
  }

  return isValidLuhn(digits);
}

function isValidLuhn(cardNumber) {
  let sum = 0;
  let shouldDouble = false;

  for (let index = cardNumber.length - 1; index >= 0; index--) {
    let digit = Number(cardNumber[index]);

    if (shouldDouble) {
      digit *= 2;

      if (digit > 9) {
        digit -= 9;
      }
    }

    sum += digit;
    shouldDouble = !shouldDouble;
  }

  return sum % 10 === 0;
}

function isValidExpiryDate(value) {
  if (!/^\d{2}\/\d{2}$/.test(value)) {
    return false;
  }

  const [monthText, yearText] = value.split("/");
  const month = Number(monthText);
  const year = Number(`20${yearText}`);

  if (month < 1 || month > 12) {
    return false;
  }

  const now = new Date();
  const currentMonth = now.getMonth() + 1;
  const currentYear = now.getFullYear();

  return year > currentYear || (year === currentYear && month >= currentMonth);
}

function isValidCvv(value) {
  return /^\d{3}$/.test(value);
}

function validatePaymentForm() {
  let isValid = true;

  clearFieldError(cardHolderInput);
  clearFieldError(cardNumberInput);
  clearFieldError(expiryDateInput);
  clearFieldError(cvvInput);

  if (!isValidCardHolder(cardHolderInput.value)) {
    setFieldError(cardHolderInput, "Kart üzerindeki ad soyad bilgisini doğru gir.");
    isValid = false;
  }

  if (!isValidTurkishCardNumber(cardNumberInput.value)) {
    setFieldError(cardNumberInput, "Kart numarası 16 haneli ve geçerli formatta olmalı.");
    isValid = false;
  }

  if (!isValidExpiryDate(expiryDateInput.value)) {
    setFieldError(expiryDateInput, "Son kullanma tarihi AA/YY formatında ve geçerli olmalı.");
    isValid = false;
  }

  if (!isValidCvv(cvvInput.value)) {
    setFieldError(cvvInput, "CVV 3 haneli olmalı.");
    isValid = false;
  }

  return isValid;
}

function showPaymentResult(type) {
  const isSuccess = type === "success";

  paymentResultModal.classList.toggle("error", !isSuccess);
  paymentResultIcon.textContent = isSuccess ? "✓" : "!";
  paymentResultTitle.textContent = isSuccess ? "Ödeme Başarılı" : "Ödeme Başarısız";
  paymentResultText.textContent = isSuccess
    ? "Siparişin başarıyla oluşturuldu. Ana sayfaya yönlendiriliyorsun..."
    : "Ödeme bilgileri doğrulanamadı. Ana sayfaya yönlendiriliyorsun...";

  paymentResultOverlay.classList.add("active");

  if (isSuccess) {
    removePurchasedItemsFromCart();
  }

  setTimeout(() => {
    window.location.href = "landingPage.html";
  }, 4000);
}

function removePurchasedItemsFromCart() {
  const cart = getCart();
  const nextCart = cart.filter((item) => !item.selected);

  saveCart(nextCart);
}

function initializePaymentInputs() {
  cardNumberInput.addEventListener("input", () => {
    cardNumberInput.value = formatCardNumber(cardNumberInput.value);
  });

  expiryDateInput.addEventListener("input", () => {
    expiryDateInput.value = formatExpiryDate(expiryDateInput.value);
  });

  cvvInput.addEventListener("input", () => {
    cvvInput.value = onlyDigits(cvvInput.value).slice(0, 3);
  });
}

function initializeCheckoutEvents() {
  changeAddressButton.addEventListener("click", () => {
    if (typeof openAddressModal === "function") {
      openAddressModal();
    }
  });

  paymentForm.addEventListener("submit", (event) => {
    event.preventDefault();

    const selectedItems = getSelectedCartItems();

    if (selectedItems.length === 0) {
      showPaymentResult("error");
      return;
    }

    const currentUser = getCurrentUser();

    if (!currentUser) {
      window.location.href = "login.html?redirect=checkout.html";
      return;
    }

    const selectedAddress =
      typeof getSelectedAddress === "function" ? getSelectedAddress() : null;

    if (!selectedAddress) {
      if (typeof openAddressModal === "function") {
        openAddressModal();
      }

      updateSelectedAddressView();
      return;
    }

    const isPaymentValid = validatePaymentForm();

    if (isPaymentValid) {
      showPaymentResult("success");
    } else {
      showPaymentResult("error");
    }
  });
}

function initializeCheckoutGuard() {
  const currentUser = getCurrentUser();

  if (!currentUser) {
    window.location.href = "login.html?redirect=checkout.html";
    return;
  }

  const selectedAddress =
    typeof getSelectedAddress === "function" ? getSelectedAddress() : null;

  if (!selectedAddress && typeof openAddressModal === "function") {
    setTimeout(() => {
      openAddressModal();
    }, 250);
  }
}

function initializeCheckoutPage() {
  renderCheckoutItems();
  updateSelectedAddressView();
  initializePaymentInputs();
  initializeCheckoutEvents();

  setTimeout(() => {
    initializeCheckoutGuard();
  }, 100);
}

document.addEventListener("DOMContentLoaded", initializeCheckoutPage);