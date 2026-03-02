function Tabs(container) {
  this.tabs = container.querySelectorAll(".app-tabs__tab");
  this.showActiveTab = container.dataset.activetabstatus || false;
  this.tabPanels = container.querySelectorAll(".app-tabs__panel");
  delete container.dataset.module;
}

Tabs.prototype.init = function () {
  if (!this.tabs) {
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
  if (this.showActiveTab && params.has(this.showActiveTab)) {
    this.tabs[params.get(this.showActiveTab)].click();
  } else if (this.showActiveTab) {
    const savedHash = sessionStorage.getItem(
      "app-tabs:" + window.location.pathname,
    );
    const savedTab =
      savedHash && Array.from(this.tabs).find((t) => t.hash === savedHash);
    (savedTab || this.tabs[0]).click();
  } else {
    this.tabs[0].click();
  }
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
  if (this.showActiveTab) {
    sessionStorage.setItem("app-tabs:" + window.location.pathname, hash);
  }
};

// Function to update the task counts – called only after content is loaded
function updateTaskCounts() {
  const todoContainer = document.querySelector(
    '#tasks-todo [data-fetch="true"]',
  );
  const doneContainer = document.querySelector(
    '#tasks-done [data-fetch="true"]',
  );

  const todoCount = todoContainer
    ? todoContainer.querySelectorAll(".app-card").length
    : 0;
  const doneCount = doneContainer
    ? doneContainer.querySelectorAll(".app-card").length
    : 0;

  const todoSpan = document.getElementById("todo-task-count");
  const doneSpan = document.getElementById("done-task-count");

  if (todoSpan) todoSpan.textContent = "(" + todoCount + ")";
  if (doneSpan) doneSpan.textContent = "(" + doneCount + ")";
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

function initBackLinks() {
  document.querySelectorAll(".js-back-link").forEach(function (link) {
    link.addEventListener("click", function (event) {
      event.preventDefault();
      history.back();
    });
  });
}

const appInit = () => {
  const appTabs = document.querySelectorAll(`[data-module="app-tabs"]`);

  if (appTabs) {
    appTabs.forEach(function (tabs) {
      new Tabs(tabs).init();
    });
  }

  initBackLinks();
  initDataFetch();
  convertMinutesToReadableDate();
  // No call to updateTaskCounts() here – avoids the (0) flash
};

appInit();
