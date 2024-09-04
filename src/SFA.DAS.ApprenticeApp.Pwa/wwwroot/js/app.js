﻿// Tabs

function Tabs(container) {
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
    //panel.hidden = true;
  });
  this.tabs.forEach((tab) => {
    tab.ariaSelected = false;
    tab.parentElement.classList.remove("app-tabs__list-item--selected");
  });
  tab.setAttribute("aria-selected", true);
  tab.parentElement.classList.add("app-tabs__list-item--selected");
  const { hash } = tab;
  const panel = document.getElementById(hash.substring(1));
  //panel.hidden = false;
};

const appTabs = document.querySelectorAll(`[data-module="app-tabs"]`);

if (appTabs) {
  appTabs.forEach(function (tabs) {
    new Tabs(tabs).init();
  });
}

// Overlays

function Overlay(link) {
  this.link = link;
  this.bodyClassName = "app-template__body--overlay-open";
  this.showOverlayClassName = "app-overlay--visible";
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
  this.overlay.classList.add(this.showOverlayClassName);
  const firstInput = this.overlay.querySelector("input");
  if (firstInput) {
    firstInput.focus();
  }
};

Overlay.prototype.hideOverlay = function (event) {
  event.preventDefault();
  document.body.classList.remove(this.bodyClassName);
  this.overlay.classList.remove(this.showOverlayClassName);
};

const appOverlays = document.querySelectorAll(`[data-module="app-overlay"]`);

if (appOverlays) {
  appOverlays.forEach(function (overlay) {
    new Overlay(overlay).init();
  });
}

// Dropdown Menus

function Dropdown(container) {
  this.toggle = container.querySelector(".app-dropdown__toggle");
  this.menu = container.querySelector(".app-dropdown__menu");
  this.showMenuClassName = "app-dropdown__menu--visible";
}

Dropdown.prototype.init = function () {
  this.toggle.addEventListener("click", this.toggleMenu.bind(this));
};

Dropdown.prototype.toggleMenu = function () {
  if (this.menu.classList.contains(this.showMenuClassName)) {
    this.menu.classList.remove(this.showMenuClassName);
  } else {
    this.menu.classList.add(this.showMenuClassName);
  }
};

const appDropdowns = document.querySelectorAll(`[data-module="app-dropdown"]`);

if (appDropdowns) {
  appDropdowns.forEach(function (dropdown) {
    new Dropdown(dropdown).init();
  });
}

// Collapse

function Collapse(container) {
  this.container = container;
  this.button = container.querySelector(".app-collapse__button");
  this.title = container.querySelector(".app-collapse__title");
  this.content = container.querySelector(".app-collapse__content");
  this.radios = container.querySelectorAll(".app-radios__input");
  this.expandedClass = "app-collapse--expanded";
}

Collapse.prototype.init = function () {
  this.button.addEventListener("click", this.toggleContent.bind(this));
  if (this.radios.length > 0) {
    this.radios.forEach((radio) => {
      if (radio.checked) {
        const labelVal = document.querySelector("label[for=" + radio.id + "]");
        this.title.innerHTML = labelVal.innerHTML;
      }
      radio.addEventListener("change", this.radioChange.bind(this));
    });
  }
};

Collapse.prototype.toggleContent = function () {
  if (this.container.classList.contains(this.expandedClass)) {
    this.button.setAttribute("aria-expanded", false);
    this.container.classList.remove(this.expandedClass);
  } else {
    this.button.setAttribute("aria-expanded", true);
    this.container.classList.add(this.expandedClass);
  }
};

Collapse.prototype.radioChange = function (event) {
  const radio = event.target;
  if (radio.checked) {
    const labelVal = document.querySelector("label[for=" + radio.id + "]");
    this.title.innerHTML = labelVal.innerHTML;
  }
};

const appCollapsable = document.querySelectorAll(
  `[data-module="app-collapse"]`,
);

if (appCollapsable) {
  appCollapsable.forEach(function (collapse) {
    new Collapse(collapse).init();
  });
}