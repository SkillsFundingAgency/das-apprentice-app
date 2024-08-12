function Tabs(container) {
  this.container = container;
  this.tabs = container.querySelectorAll(".app-tabs__tab");
  this.tabPanels = container.querySelectorAll(".app-tabs__panel");
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
  panel.hidden = false;
};

const appTabs = document.querySelectorAll(`[data-module="app-tabs"]`);

if (appTabs) {
  appTabs.forEach(function (tabs) {
    new Tabs(tabs).init();
  });
}
