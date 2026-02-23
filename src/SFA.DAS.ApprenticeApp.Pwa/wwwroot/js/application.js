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
};

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
      })
      .catch(function (error) {
        element.innerHTML =
          "<p class='govuk-error-message'>Failed to load content</p>";
        console.error("Data fetch error:", error);
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
};

appInit();
