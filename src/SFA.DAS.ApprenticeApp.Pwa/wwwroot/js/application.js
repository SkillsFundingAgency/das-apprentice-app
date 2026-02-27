// Cookie helpers
function setCookie(name, value, days) {
  let expires = "";
  if (days) {
    const date = new Date();
    date.setTime(date.getTime() + (days * 24 * 60 * 60 * 1000));
    expires = "; expires=" + date.toUTCString();
  }
  document.cookie = name + "=" + (value || "") + expires + "; path=/";
}

function getCookie(name) {
  const nameEQ = name + "=";
  const ca = document.cookie.split(';');
  for (let i = 0; i < ca.length; i++) {
    let c = ca[i];
    while (c.charAt(0) === ' ') c = c.substring(1, c.length);
    if (c.indexOf(nameEQ) === 0) return c.substring(nameEQ.length, c.length);
  }
  return null;
}

function Tabs(container) {
  this.tabs = container.querySelectorAll(".app-tabs__tab");
  this.showActiveTab = container.dataset.activetabstatus || false;
  this.tabPanels = container.querySelectorAll(".app-tabs__panel");
  this.cookieName = container.dataset.cookie;  // read cookie name if present
  delete container.dataset.module;
}

Tabs.prototype.init = function () {
  if (!this.tabs || this.tabs.length === 0) {
    return;
  }

  this.tabs.forEach((tab) => {
    tab.setAttribute("role", "tab");
    tab.addEventListener("click", this.handleTabClick.bind(this));
  });
  this.tabPanels.forEach((tab) => {
    tab.setAttribute("role", "tabpanel");
  });

  const params = new URLSearchParams(window.location.search);
  let tabToOpen = null;

  // 1. URL parameter (if configured) takes precedence
  if (this.showActiveTab && params.has(this.showActiveTab)) {
    const index = parseInt(params.get(this.showActiveTab), 10);
    if (!isNaN(index) && index >= 0 && index < this.tabs.length) {
      tabToOpen = this.tabs[index];
    }
  }

  // 2. Cookie (if cookieName is set)
  if (!tabToOpen && this.cookieName) {
    const savedHash = getCookie(this.cookieName);
    if (savedHash) {
      tabToOpen = Array.from(this.tabs).find(t => t.hash === savedHash);
    }
  }

  // 3. Fallback to first tab
  if (!tabToOpen) {
    tabToOpen = this.tabs[0];
  }

  tabToOpen.click();
};

Tabs.prototype.handleTabClick = function (event) {
  event.preventDefault();
  const tab = event.target;

  this.tabPanels.forEach(function (panel) {
    panel.hidden = true;
  });
  this.tabs.forEach((tab) => {
    tab.ariaSelected = false;
    tab.parentElement.classList.remove("app-tabs__list-item--selected");
  });

  tab.setAttribute("aria-selected", true);
  tab.parentElement.classList.add("app-tabs__list-item--selected");

  const { hash } = tab;
  const panel = document.getElementById(hash.substring(1));
  if (panel) {
    panel.hidden = false;
  }

  // Save to cookie if cookieName is set
  if (this.cookieName) {
    setCookie(this.cookieName, hash, 30); // expires in 30 days
  }
  // (Optional: you could keep sessionStorage as a fallback if cookieName is not set,
  // but here we've removed it for clarity. If you need both, let me know.)
};

// Function to update the task counts – called only after content is loaded
function updateTaskCounts() {
  const todoContainer = document.querySelector('#tasks-todo [data-fetch="true"]');
  const doneContainer = document.querySelector('#tasks-done [data-fetch="true"]');

  const todoCount = todoContainer ? todoContainer.querySelectorAll('.app-card').length : 0;
  const doneCount = doneContainer ? doneContainer.querySelectorAll('.app-card').length : 0;

  const todoSpan = document.getElementById('todo-task-count');
  const doneSpan = document.getElementById('done-task-count');

  if (todoSpan) todoSpan.textContent = '(' + todoCount + ')';
  if (doneSpan) doneSpan.textContent = '(' + doneCount + ')';
}

function initDataFetch() {
  const elements = document.querySelectorAll('[data-fetch="true"][data-url]');

  elements.forEach(function (element) {
    const url = element.dataset.url;

    if (!url) {
      return;
    }

    fetch(url)
        .then(function (response) {
          if (!response.ok) {
            throw new Error("Network response was not ok");
          }
          return response.text();
        })
        .then(function (html) {
          element.innerHTML = html;
          convertMinutesToReadableDate();
          updateTaskCounts(); // <-- count appears here, never shows (0) before data loads
        })
        .catch(function (error) {
          element.innerHTML =
              "<p class='govuk-error-message'>Failed to load content</p>";
          console.error("Data fetch error:", error);
          updateTaskCounts(); // optionally show (0) on error
        });
  });
}

const convertMinutesToReadableDate = () => {
  const dates = document.querySelectorAll(`.app-js-convert-minutes-to-date`);
  dates.forEach(function (element) {
    const dueDateTime = new Date(element.dataset.due);
    const minutes = parseInt(element.dataset.val, 10);
    const date = new Date(dueDateTime.getTime() - minutes * 60 * 1000);
    element.innerHTML = date.toLocaleDateString("en-GB", {
      year: "numeric",
      month: "long",
      day: "numeric",
      hour: "numeric",
      minute: "numeric",
      hour12: true,
    });
  });
};

const appInit = () => {
  const appTabs = document.querySelectorAll(`[data-module="app-tabs"]`);

  if (appTabs) {
    appTabs.forEach(function (tabs) {
      new Tabs(tabs).init();
    });
  }

  initDataFetch();
  convertMinutesToReadableDate();
  // No call to updateTaskCounts() here – avoids the (0) flash
};

appInit();