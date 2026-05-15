const ADDRESS_STORAGE_KEY = "shopsphere_addresses";
const SELECTED_ADDRESS_KEY = "shopsphere_selected_address_id";

function getAddresses() {
  try {
    return JSON.parse(localStorage.getItem(ADDRESS_STORAGE_KEY) || "[]");
  } catch {
    return [];
  }
}

function saveAddresses(addresses) {
  localStorage.setItem(ADDRESS_STORAGE_KEY, JSON.stringify(addresses));
}

function getSelectedAddressId() {
  return localStorage.getItem(SELECTED_ADDRESS_KEY);
}

function setSelectedAddressId(id) {
  localStorage.setItem(SELECTED_ADDRESS_KEY, String(id));
}

function getSelectedAddress() {
  const addresses = getAddresses();
  const selectedId = getSelectedAddressId();

  return addresses.find((address) => String(address.id) === String(selectedId)) || null;
}

function shortenAddress(addressText) {
  if (!addressText) return "Adres Ekle";

  return addressText.length > 22
    ? `${addressText.slice(0, 22)}...`
    : addressText;
}

function updateHeaderAddressText() {
  const selectedAddress = getSelectedAddress();
  const addressTexts = document.querySelectorAll(".address-box strong");

  addressTexts.forEach((textElement) => {
    textElement.textContent = selectedAddress
      ? shortenAddress(selectedAddress.fullAddress)
      : "Adres Ekle";
  });
}

function createAddressModal() {
  const existingModal = document.getElementById("addressModalOverlay");

  if (existingModal) return;

  const modalHTML = `
    <div id="addressModalOverlay" class="address-modal-overlay">
      <section class="address-modal" role="dialog" aria-modal="true" aria-labelledby="addressModalTitle">

        <div class="address-modal-header">
          <h2 id="addressModalTitle">Teslimat Adresi</h2>
          <button id="addressModalClose" class="address-modal-close" type="button" aria-label="Adresi kapat">
            ×
          </button>
        </div>

        <div class="address-modal-body">
          <p class="address-section-title">Kayıtlı Adreslerim</p>

          <div id="savedAddressList" class="saved-address-list"></div>

          <div id="emptyAddressBox" class="empty-address-box">
            Henüz kayıtlı adresin yok. Yeni adres ekleyerek devam edebilirsin.
          </div>

          <div class="address-divider"></div>

          <button id="newAddressToggle" class="new-address-toggle" type="button">
            + Yeni Adres Ekle
          </button>

          <form id="newAddressForm" class="new-address-form" novalidate>
            <div class="address-form-group">
              <label for="addressTitle">Adres Başlığı</label>
              <input 
                id="addressTitle" 
                type="text" 
                placeholder="Ev, okul, iş yeri..."
              />
              <span class="address-form-hint">Bu alan isteğe bağlıdır.</span>
            </div>

            <div class="address-form-group">
              <label for="fullAddress">Açık Adres</label>
              <textarea 
                id="fullAddress" 
                placeholder="Açık adresini tek metin olarak yaz..."
                required
              ></textarea>
              <p id="addressError" class="address-error"></p>
            </div>
          </form>
        </div>

        <div class="address-modal-footer">
          <button id="addressCancelButton" class="address-secondary-button" type="button">
            Vazgeç
          </button>

          <button id="addressSaveButton" class="address-primary-button" type="button">
            Adresi Kaydet
          </button>
        </div>

      </section>
    </div>
  `;

  document.body.insertAdjacentHTML("beforeend", modalHTML);
}

function renderSavedAddresses() {
  const savedAddressList = document.getElementById("savedAddressList");
  const emptyAddressBox = document.getElementById("emptyAddressBox");

  const addresses = getAddresses();
  const selectedId = getSelectedAddressId();

  if (!savedAddressList || !emptyAddressBox) return;

  if (addresses.length === 0) {
    savedAddressList.innerHTML = "";
    emptyAddressBox.style.display = "block";
    return;
  }

  emptyAddressBox.style.display = "none";

  savedAddressList.innerHTML = addresses
    .map((address) => {
      const isSelected = String(address.id) === String(selectedId);

      return `
        <label class="saved-address-item ${isSelected ? "selected" : ""}" data-id="${address.id}">
          <input 
            type="radio" 
            name="selectedAddress" 
            value="${address.id}"
            ${isSelected ? "checked" : ""}
          />

          <strong>${escapeHTML(address.title || "Adres")}</strong>
          <span>${escapeHTML(address.fullAddress)}</span>

          <div class="saved-address-actions">
            <button 
              class="address-small-button delete" 
              type="button" 
              data-delete-id="${address.id}"
            >
              Sil
            </button>
          </div>
        </label>
      `;
    })
    .join("");
}

function escapeHTML(value) {
  return String(value)
    .replaceAll("&", "&amp;")
    .replaceAll("<", "&lt;")
    .replaceAll(">", "&gt;")
    .replaceAll('"', "&quot;")
    .replaceAll("'", "&#039;");
}

function openAddressModal() {
  const overlay = document.getElementById("addressModalOverlay");
  const newAddressForm = document.getElementById("newAddressForm");
  const addressError = document.getElementById("addressError");

  if (!overlay) return;

  renderSavedAddresses();

  overlay.classList.add("active");
  document.body.style.overflow = "hidden";

  if (addressError) addressError.textContent = "";

  const addresses = getAddresses();

  if (addresses.length === 0 && newAddressForm) {
    newAddressForm.classList.add("active");
  }
}

function closeAddressModal() {
  const overlay = document.getElementById("addressModalOverlay");

  if (!overlay) return;

  overlay.classList.remove("active");
  document.body.style.overflow = "";
}

function toggleNewAddressForm() {
  const form = document.getElementById("newAddressForm");

  if (!form) return;

  form.classList.toggle("active");
}

function saveNewAddress() {
  const addressTitle = document.getElementById("addressTitle");
  const fullAddress = document.getElementById("fullAddress");
  const addressError = document.getElementById("addressError");

  if (!fullAddress || !addressError) return false;

  const titleValue = addressTitle.value.trim();
  const fullAddressValue = fullAddress.value.trim();

  addressError.textContent = "";

  if (!fullAddressValue) {
    addressError.textContent = "Açık adres alanı zorunludur.";
    return false;
  }

  if (fullAddressValue.length < 10) {
    addressError.textContent = "Lütfen daha açıklayıcı bir adres gir.";
    return false;
  }

  const addresses = getAddresses();

  const newAddress = {
    id: Date.now(),
    title: titleValue || `Adres ${addresses.length + 1}`,
    fullAddress: fullAddressValue
  };

  addresses.push(newAddress);
  saveAddresses(addresses);
  setSelectedAddressId(newAddress.id);

  addressTitle.value = "";
  fullAddress.value = "";

  const form = document.getElementById("newAddressForm");
  form?.classList.remove("active");

  renderSavedAddresses();
  updateHeaderAddressText();

  return true;
}

function selectSavedAddress(addressId) {
  setSelectedAddressId(addressId);
  renderSavedAddresses();
  updateHeaderAddressText();
}

function deleteSavedAddress(addressId) {
  const addresses = getAddresses();
  const selectedId = getSelectedAddressId();

  const nextAddresses = addresses.filter((address) => {
    return String(address.id) !== String(addressId);
  });

  saveAddresses(nextAddresses);

  if (String(selectedId) === String(addressId)) {
    if (nextAddresses.length > 0) {
      setSelectedAddressId(nextAddresses[0].id);
    } else {
      localStorage.removeItem(SELECTED_ADDRESS_KEY);
    }
  }

  renderSavedAddresses();
  updateHeaderAddressText();
}

function initializeAddressModalEvents() {
  const overlay = document.getElementById("addressModalOverlay");
  const closeButton = document.getElementById("addressModalClose");
  const cancelButton = document.getElementById("addressCancelButton");
  const saveButton = document.getElementById("addressSaveButton");
  const newAddressToggle = document.getElementById("newAddressToggle");
  const savedAddressList = document.getElementById("savedAddressList");

  document.querySelectorAll(".address-box").forEach((button) => {
    button.addEventListener("click", openAddressModal);
  });

  closeButton?.addEventListener("click", closeAddressModal);
  cancelButton?.addEventListener("click", closeAddressModal);
  newAddressToggle?.addEventListener("click", toggleNewAddressForm);

  saveButton?.addEventListener("click", () => {
    const form = document.getElementById("newAddressForm");

    if (form?.classList.contains("active")) {
      const saved = saveNewAddress();

      if (saved) {
        closeAddressModal();
      }

      return;
    }

    const selectedRadio = document.querySelector("input[name='selectedAddress']:checked");

    if (selectedRadio) {
      selectSavedAddress(selectedRadio.value);
      closeAddressModal();
    } else {
      toggleNewAddressForm();
    }
  });

  savedAddressList?.addEventListener("change", (event) => {
    if (event.target.name === "selectedAddress") {
      selectSavedAddress(event.target.value);
    }
  });

  savedAddressList?.addEventListener("click", (event) => {
    const deleteButton = event.target.closest("[data-delete-id]");

    if (!deleteButton) return;

    event.preventDefault();
    event.stopPropagation();

    deleteSavedAddress(deleteButton.dataset.deleteId);
  });

  overlay?.addEventListener("click", (event) => {
    if (event.target === overlay) {
      closeAddressModal();
    }
  });

  document.addEventListener("keydown", (event) => {
    if (event.key === "Escape") {
      closeAddressModal();
    }
  });
}

function initializeAddressModal() {
  createAddressModal();
  initializeAddressModalEvents();
  updateHeaderAddressText();
}

document.addEventListener("DOMContentLoaded", initializeAddressModal);