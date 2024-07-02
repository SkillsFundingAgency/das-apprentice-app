const notificationDisplaySettings = () => {
  const notificationsSwitch = document.getElementById("notificationSwitch");
  if (Notification.permission === "granted") {
    const notifyOn = document.getElementById("notificationsOn");
    if (notifyOn) {
      notifyOn.style.display = "block";
    }
    const notifyOff = document.getElementById("notificationsOff");
    if (notifyOff) {
      notifyOff.style.display = "none";
    }

    notificationsSwitch.checked = true;
  }

  if (Notification.permission === "denied") {
    const notifyOn = document.getElementById("notificationsOn");
    if (notifyOn) {
      notifyOn.style.display = "none";
    }
    const notifyOff = document.getElementById("notificationsOff");
    if (notifyOff) {
      notifyOff.style.display = "block";
    }

    notificationsSwitch.checked = false;
  }
};

notificationDisplaySettings();
function toggleNotifications() {
  const notificationsSwitch = document.getElementById("notificationSwitch");
  if (notificationsSwitch.checked) {
    notificationsSwitch.checked = false;
  } else {
    notificationsSwitch.checked = true;
  }

  const toggleNotificationElements = document.querySelectorAll(
    ".change-notifications"
  );

  for (const element of toggleNotificationElements) {
    element.classList.toggle("app-modal--visible");
  }
  notificationDisplaySettings();
}
