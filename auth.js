function getUsers() {
  return JSON.parse(localStorage.getItem("shopsphere_users") || "[]");
}

function saveUsers(users) {
  localStorage.setItem("shopsphere_users", JSON.stringify(users));
}

function setFieldError(input, message) {
  const formGroup = input.closest(".form-group");
  const error = formGroup?.querySelector(".error-message");

  if (error) {
    error.textContent = message;
  }

  input.classList.toggle("has-error", Boolean(message));
}

function clearFieldError(input) {
  setFieldError(input, "");
}

function isValidEmail(email) {
  return /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email);
}

function isValidPassword(password) {
  const hasLength = password.length >= 8 && password.length <= 15;
  const hasNumber = /\d/.test(password);
  const hasUpper = /[A-ZÇĞİÖŞÜ]/.test(password);
  const hasLower = /[a-zçğıöşü]/.test(password);

  return hasLength && hasNumber && hasUpper && hasLower;
}

function initializePasswordToggles() {
  const toggleButtons = document.querySelectorAll(".toggle-password");

  toggleButtons.forEach((button) => {
    button.addEventListener("click", () => {
      const input = button.previousElementSibling;

      if (!input) return;

      const isPassword = input.type === "password";
      input.type = isPassword ? "text" : "password";
      button.textContent = isPassword ? "🙈" : "👁";
    });
  });
}

function initializeRegisterForm() {
  const form = document.getElementById("registerForm");

  if (!form) return;

  const message = document.getElementById("registerMessage");
  const agreementError = document.getElementById("agreementError");

  form.addEventListener("submit", (event) => {
    event.preventDefault();

    const firstName = document.getElementById("firstName");
    const lastName = document.getElementById("lastName");
    const email = document.getElementById("registerEmail");
    const password = document.getElementById("registerPassword");
    const agreement = document.getElementById("agreement");
    const selectedRole = document.querySelector("input[name='role']:checked");

    let isValid = true;

    [firstName, lastName, email, password].forEach(clearFieldError);
    agreementError.textContent = "";
    message.textContent = "";
    message.className = "form-message";

    if (!firstName.value.trim()) {
      setFieldError(firstName, "Ad alanı zorunludur.");
      isValid = false;
    }

    if (!lastName.value.trim()) {
      setFieldError(lastName, "Soyad alanı zorunludur.");
      isValid = false;
    }

    if (!email.value.trim()) {
      setFieldError(email, "E-posta alanı zorunludur.");
      isValid = false;
    } else if (!isValidEmail(email.value.trim())) {
      setFieldError(email, "Geçerli bir e-posta adresi gir.");
      isValid = false;
    }

    if (!password.value.trim()) {
      setFieldError(password, "Şifre alanı zorunludur.");
      isValid = false;
    } else if (!isValidPassword(password.value.trim())) {
      setFieldError(password, "Şifre kurallara uygun değil.");
      isValid = false;
    }

    if (!agreement.checked) {
      agreementError.textContent = "Devam etmek için üyelik sözleşmesini kabul etmelisin.";
      isValid = false;
    }

    const users = getUsers();
    const emailExists = users.some(
      (user) => user.email.toLowerCase() === email.value.trim().toLowerCase()
    );

    if (emailExists) {
      setFieldError(email, "Bu e-posta adresiyle kayıtlı bir kullanıcı var.");
      isValid = false;
    }

    if (!isValid) return;

    const newUser = {
      id: Date.now(),
      firstName: firstName.value.trim(),
      lastName: lastName.value.trim(),
      email: email.value.trim(),
      password: password.value.trim(),
      role: selectedRole?.value || "Customer"
    };

    users.push(newUser);
    saveUsers(users);

    message.textContent = "Üyelik oluşturuldu. Giriş sayfasına yönlendiriliyorsun...";
    message.classList.add("success");

    setTimeout(() => {
      window.location.href = "login.html";
    }, 900);
  });
}

function initializeLoginForm() {
  const form = document.getElementById("loginForm");

  if (!form) return;

  const message = document.getElementById("loginMessage");

  form.addEventListener("submit", (event) => {
    event.preventDefault();

    const email = document.getElementById("loginEmail");
    const password = document.getElementById("loginPassword");

    [email, password].forEach(clearFieldError);
    message.textContent = "";
    message.className = "form-message";

    let isValid = true;

    if (!email.value.trim()) {
      setFieldError(email, "E-posta alanı zorunludur.");
      isValid = false;
    } else if (!isValidEmail(email.value.trim())) {
      setFieldError(email, "Geçerli bir e-posta adresi gir.");
      isValid = false;
    }

    if (!password.value.trim()) {
      setFieldError(password, "Şifre alanı zorunludur.");
      isValid = false;
    }

    if (!isValid) return;

    const users = getUsers();

    const foundUser = users.find(
      (user) =>
        user.email.toLowerCase() === email.value.trim().toLowerCase() &&
        user.password === password.value.trim()
    );

    if (!foundUser) {
      message.textContent = "E-posta veya şifre hatalı.";
      message.classList.add("error");
      return;
    }

    localStorage.setItem(
      "shopsphere_current_user",
      JSON.stringify({
        id: foundUser.id,
        firstName: foundUser.firstName,
        lastName: foundUser.lastName,
        email: foundUser.email,
        role: foundUser.role
      })
    );

    message.textContent = "Giriş başarılı. Ana sayfaya yönlendiriliyorsun...";
    message.classList.add("success");

    setTimeout(() => {
      window.location.href = "landingPage.html";
    }, 800);
  });
}

initializePasswordToggles();
initializeRegisterForm();
initializeLoginForm();