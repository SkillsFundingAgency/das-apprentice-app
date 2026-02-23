// --- Push Notifications ---

let currentEndpoint = "";

const urlB64ToUint8Array = (base64String) => {
  const padding = "=".repeat((4 - (base64String.length % 4)) % 4);
  const base64 = (base64String + padding).replace(/-/g, "+").replace(/_/g, "/");
  const rawData = window.atob(base64);
  const outputArray = new Uint8Array(rawData.length);
  for (let i = 0; i < rawData.length; ++i) {
    outputArray[i] = rawData.charCodeAt(i);
  }
  return outputArray;
};

const arrayBufferToBase64 = (buffer) => {
  let binary = "";
  const bytes = new Uint8Array(buffer);
  for (let i = 0; i < bytes.byteLength; i++) {
    binary += String.fromCharCode(bytes[i]);
  }
  return window.btoa(binary);
};

const sendSubscriptionToServer = (subscription, isSubscribed) => {
  return fetch("/Profile/AddSubscription", {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify({
      endpoint: subscription.endpoint,
      publicKey: arrayBufferToBase64(subscription.getKey("p256dh")),
      authenticationSecret: arrayBufferToBase64(subscription.getKey("auth")),
      isSubscribed: isSubscribed,
    }),
  }).then((response) => {
    if (!response.ok) throw new Error("Server error adding subscription.");
  });
};

const removeSubscriptionFromServer = (endpoint) => {
  return fetch("/Profile/RemoveSubscription", {
    method: "DELETE",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify({ endpoint }),
  }).then((response) => {
    if (!response.ok) throw new Error("Server error removing subscription.");
  });
};

const registerPush = async (registration) => {
  const applicationServerKey = urlB64ToUint8Array(window.pushNotificationKey);
  const subscription = await registration.pushManager.subscribe({
    userVisibleOnly: true,
    applicationServerKey,
  });
  currentEndpoint = subscription.endpoint;
  await sendSubscriptionToServer(subscription, false);
};

const showNotificationHint = (message) => {
  const hint = document.getElementById("notificationHint");
  if (!hint) return;
  hint.textContent = message;
  hint.hidden = false;
};

const hideNotificationHint = () => {
  const hint = document.getElementById("notificationHint");
  if (hint) hint.hidden = true;
};

const updateToggleState = async (toggle) => {
  if (Notification.permission === "granted") {
    const registration = await navigator.serviceWorker.getRegistration();
    if (registration) {
      const subscription = await registration.pushManager.getSubscription();
      toggle.checked = !!subscription;
      if (subscription) currentEndpoint = subscription.endpoint;
    } else {
      toggle.checked = false;
    }
  } else {
    toggle.checked = false;
  }
};

const enableNotifications = async (toggle) => {
  hideNotificationHint();
  let permission = Notification.permission;

  if (permission === "default") {
    permission = await Notification.requestPermission();
  }

  if (permission === "granted") {
    try {
      const registration = await navigator.serviceWorker.ready;
      await registerPush(registration);
      toggle.checked = true;
    } catch (e) {
      console.error("Failed to subscribe:", e);
      toggle.checked = false;
      showNotificationHint(
        "There was a problem turning on push notifications. Please try again."
      );
    }
  } else {
    toggle.checked = false;
    showNotificationHint(
      "To turn on push notifications, allow notifications for this site in your browser settings."
    );
  }
};

const disableNotifications = async (toggle) => {
  hideNotificationHint();
  try {
    const registration = await navigator.serviceWorker.getRegistration();
    if (registration) {
      const subscription = await registration.pushManager.getSubscription();
      if (subscription) {
        currentEndpoint = subscription.endpoint;
        await subscription.unsubscribe();
        await removeSubscriptionFromServer(currentEndpoint);
      }
    }
    toggle.checked = false;
  } catch (e) {
    console.error("Failed to unsubscribe:", e);
    toggle.checked = true;
    showNotificationHint(
      "There was a problem turning off push notifications. Please try again."
    );
  }
};

const initPushNotifications = () => {
  const toggle = document.getElementById("notificationSwitch");
  if (!toggle) return;

  if (!("Notification" in window) || !("serviceWorker" in navigator)) {
    toggle.disabled = true;
    return;
  }

  updateToggleState(toggle);

  toggle.addEventListener("change", async () => {
    if (toggle.checked) {
      await enableNotifications(toggle);
    } else {
      await disableNotifications(toggle);
    }
  });

  // Keep toggle in sync if the user changes permission via browser settings
  navigator.permissions
    .query({ name: "notifications" })
    .then((status) => {
      status.onchange = () => updateToggleState(toggle);
    });
};

// --- Cookies ---

const COOKIE_TRACK_NAME = "SFA.ApprenticeApp.CookieTrack";

const getCookie = (name) => {
  const nameEQ = name + "=";
  for (let cookie of document.cookie.split(";")) {
    cookie = cookie.trimStart();
    if (cookie.startsWith(nameEQ)) {
      return decodeURIComponent(cookie.substring(nameEQ.length));
    }
  }
  return null;
};

const setCookie = (name, value, expDays) => {
  let cookieString = `${name}=${value}; path=/; SameSite=None`;
  if (expDays) {
    const date = new Date();
    date.setTime(date.getTime() + expDays * 24 * 60 * 60 * 1000);
    cookieString += `; expires=${date.toUTCString()}`;
  }
  cookieString += "; Secure";
  document.cookie = cookieString;
};

// --- Additional Cookies ---

const initAdditionalCookies = () => {
  const toggle = document.getElementById("additionalCookiesSwitch");
  if (!toggle) return;

  toggle.checked = getCookie(COOKIE_TRACK_NAME) === "1";

  toggle.addEventListener("change", () => {
    setCookie(COOKIE_TRACK_NAME, toggle.checked ? "1" : "0", 999);
  });
};

// --- Text Size ---

const TEXT_SIZE_COOKIE = "SFA.ApprenticeApp.TextSize";
const TEXT_SIZE_MIN = 0.8;
const TEXT_SIZE_MAX = 1.5;
const TEXT_SIZE_STEP = 0.1;
const TEXT_SIZE_DEFAULT = 1.0;

const getSavedTextSize = () => {
  const saved = getCookie(TEXT_SIZE_COOKIE);
  if (saved) {
    const parsed = parseFloat(saved);
    if (!isNaN(parsed)) {
      return Math.max(TEXT_SIZE_MIN, Math.min(TEXT_SIZE_MAX, parsed));
    }
  }
  return TEXT_SIZE_DEFAULT;
};

const applyTextSize = (size, display) => {
  size = Math.max(TEXT_SIZE_MIN, Math.min(TEXT_SIZE_MAX, size));
  document.documentElement.style.fontSize = `${size * 100}%`;
  display.textContent = `${Math.round(size * 100)}%`;
  setCookie(TEXT_SIZE_COOKIE, size, 365 * 10);
  return size;
};

const initTextSize = () => {
  const decrease = document.getElementById("textSizeDecrease");
  const increase = document.getElementById("textSizeIncrease");
  const display = document.getElementById("textSizeDisplay");
  if (!decrease || !increase || !display) return;

  let currentSize = getSavedTextSize();
  display.textContent = `${Math.round(currentSize * 100)}%`;

  decrease.addEventListener("click", () => {
    currentSize = applyTextSize(currentSize - TEXT_SIZE_STEP, display);
  });

  increase.addEventListener("click", () => {
    currentSize = applyTextSize(currentSize + TEXT_SIZE_STEP, display);
  });
};

// --- Init ---

const settingsInit = () => {
  initPushNotifications();
  initAdditionalCookies();
  initTextSize();
};

settingsInit();
