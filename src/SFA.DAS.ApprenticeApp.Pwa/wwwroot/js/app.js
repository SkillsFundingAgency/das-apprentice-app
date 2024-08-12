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

function Overlay(link) {
  this.link = link;
  this.bodyClassName = "app-template__body--overlay-open";
  this.overlay;
}

Overlay.prototype.init = function () {
  const target = this.link.hash;
  this.overlay = document.getElementById(target.substring(1));
  if (!this.overlay) {
    return;
  }
  this.setupEvents();
};

Overlay.prototype.setupEvents = function () {
  this.link.addEventListener("click", this.showOverlay.bind(this));
  const close = this.overlay.querySelector(".app-overlay-header__link");
  close.addEventListener("click", this.hideOverlay.bind(this));
};

Overlay.prototype.showOverlay = function (event) {
  event.preventDefault();
  document.body.classList.add(this.bodyClassName);
  const firstInput = this.overlay.querySelector("input");
  console.log(firstInput);
  if (firstInput) {
    firstInput.focus();
  }
};

Overlay.prototype.hideOverlay = function () {
  document.body.classList.remove(this.bodyClassName);
};

const appOverlays = document.querySelectorAll(`[data-module="app-overlay"]`);

if (appOverlays) {
  appOverlays.forEach(function (overlay) {
    new Overlay(overlay).init();
  });
}
