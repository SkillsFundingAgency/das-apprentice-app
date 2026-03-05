(() => {
    "use strict";

    const SVG_CALENDAR_TODAY = '<svg width="32" height="35" viewBox="0 0 32 35" fill="none" xmlns="http://www.w3.org/2000/svg"><path d="M8.7373 0.0126953V3.5127H22.7627V0.0126953H26.2373V3.5127H28C28.9592 3.5127 29.7798 3.85402 30.4629 4.53711C31.146 5.2202 31.4873 6.04084 31.4873 7V31.5C31.4873 32.4592 31.146 33.2798 30.4629 33.9629C29.7798 34.646 28.9592 34.9873 28 34.9873H3.5C2.54084 34.9873 1.7202 34.646 1.03711 33.9629C0.354018 33.2798 0.0126953 32.4592 0.0126953 31.5V7C0.0126953 6.04084 0.354018 5.2202 1.03711 4.53711C1.7202 3.85402 2.54084 3.5127 3.5 3.5127H5.2627V0.0126953H8.7373ZM3.4873 31.5127H28.0127V13.9873H3.4873V31.5127ZM8.75 24.5127C9.24269 24.5127 9.65532 24.6788 9.98828 25.0117C10.3212 25.3447 10.4873 25.7573 10.4873 26.25C10.4873 26.7427 10.3212 27.1553 9.98828 27.4883C9.65532 27.8212 9.24269 27.9873 8.75 27.9873C8.25731 27.9873 7.84468 27.8212 7.51172 27.4883C7.17876 27.1553 7.0127 26.7427 7.0127 26.25C7.0127 25.7573 7.17876 25.3447 7.51172 25.0117C7.84468 24.6788 8.25731 24.5127 8.75 24.5127ZM15.75 24.5127C16.2427 24.5127 16.6553 24.6788 16.9883 25.0117C17.3212 25.3447 17.4873 25.7573 17.4873 26.25C17.4873 26.7427 17.3212 27.1553 16.9883 27.4883C16.6553 27.8212 16.2427 27.9873 15.75 27.9873C15.2573 27.9873 14.8447 27.8212 14.5117 27.4883C14.1788 27.1553 14.0127 26.7427 14.0127 26.25C14.0127 25.7573 14.1788 25.3447 14.5117 25.0117C14.8447 24.6788 15.2573 24.5127 15.75 24.5127ZM22.75 24.5127C23.2427 24.5127 23.6553 24.6788 23.9883 25.0117C24.3212 25.3447 24.4873 25.7573 24.4873 26.25C24.4873 26.7427 24.3212 27.1553 23.9883 27.4883C23.6553 27.8212 23.2427 27.9873 22.75 27.9873C22.2573 27.9873 21.8447 27.8212 21.5117 27.4883C21.1788 27.1553 21.0127 26.7427 21.0127 26.25C21.0127 25.7573 21.1788 25.3447 21.5117 25.0117C21.8447 24.6788 22.2573 24.5127 22.75 24.5127ZM8.75 17.5127C9.24269 17.5127 9.65532 17.6788 9.98828 18.0117C10.3212 18.3447 10.4873 18.7573 10.4873 19.25C10.4873 19.7427 10.3212 20.1553 9.98828 20.4883C9.65532 20.8212 9.24269 20.9873 8.75 20.9873C8.25731 20.9873 7.84468 20.8212 7.51172 20.4883C7.17876 20.1553 7.0127 19.7427 7.0127 19.25C7.0127 18.7573 7.17876 18.3447 7.51172 18.0117C7.84468 17.6788 8.25731 17.5127 8.75 17.5127ZM15.75 17.5127C16.2427 17.5127 16.6553 17.6788 16.9883 18.0117C17.3212 18.3447 17.4873 18.7573 17.4873 19.25C17.4873 19.7427 17.3212 20.1553 16.9883 20.4883C16.6553 20.8212 16.2427 20.9873 15.75 20.9873C15.2573 20.9873 14.8447 20.8212 14.5117 20.4883C14.1788 20.1553 14.0127 19.7427 14.0127 19.25C14.0127 18.7573 14.1788 18.3447 14.5117 18.0117C14.8447 17.6788 15.2573 17.5127 15.75 17.5127ZM22.75 17.5127C23.2427 17.5127 23.6553 17.6788 23.9883 18.0117C24.3212 18.3447 24.4873 18.7573 24.4873 19.25C24.4873 19.7427 24.3212 20.1553 23.9883 20.4883C23.6553 20.8212 23.2427 20.9873 22.75 20.9873C22.2573 20.9873 21.8447 20.8212 21.5117 20.4883C21.1788 20.1553 21.0127 19.7427 21.0127 19.25C21.0127 18.7573 21.1788 18.3447 21.5117 18.0117C21.8447 17.6788 22.2573 17.5127 22.75 17.5127Z" fill="#1D70B8" stroke="#1D70B8" stroke-width="0.025"/></svg>';
    const SVG_CHEVRON_LEFT = '<svg focusable="false" class="ds_icon" aria-hidden="true" role="img" xmlns="http://www.w3.org/2000/svg" height="24px" viewBox="0 0 24 24" width="24px" fill="#000000"><path d="M0 0h24v24H0z" fill="none"/><path d="M15.41 7.41L14 6l-6 6 6 6 1.41-1.41L10.83 12z"/></svg>';
    const SVG_CHEVRON_RIGHT = '<svg focusable="false" class="ds_icon" aria-hidden="true" role="img" xmlns="http://www.w3.org/2000/svg" height="24px" viewBox="0 0 24 24" width="24px" fill="#000000"><path d="M0 0h24v24H0z" fill="none"/><path d="M10 6L8.59 7.41 13.17 12l-4.58 4.59L10 18l6-6z"/></svg>';
    const SVG_DOUBLE_CHEVRON_LEFT = '<svg focusable="false" class="ds_icon" aria-hidden="true" role="img" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24"><path d="M19 16.6 17.6 18l-6-6 6-6L19 7.4 14.4 12l4.6 4.6Zm-6.6 0L11 18l-6-6 6-6 1.4 1.4L7.8 12l4.6 4.6Z"/></svg>';
    const SVG_DOUBLE_CHEVRON_RIGHT = '<svg focusable="false" class="ds_icon" aria-hidden="true" role="img" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24"><path d="M9.6 12 5 7.4 6.4 6l6 6-6 6L5 16.6 9.6 12Zm6.6 0-4.6-4.6L13 6l6 6-6 6-1.4-1.4 4.6-4.6Z"/></svg>';

    class CalendarDay {
        constructor(button, index, row, column, picker) {
            this.index = index;
            this.row = row;
            this.column = column;
            this.button = button;
            this.picker = picker;
            this.date = new Date();
        }

        init() {
            this.button.addEventListener("keydown", this.keyPress.bind(this));
            this.button.addEventListener("click", this.click.bind(this));
        }

        update(date, isHidden, isDisabled) {
            this.date = new Date(date);
            this.button.innerHTML = date.getDate();
            this.button.setAttribute("aria-label", this.picker.formattedDateHuman(this.date));

            if (isDisabled) {
                this.button.setAttribute("aria-disabled", true);
            } else {
                this.button.removeAttribute("aria-disabled");
            }

            if (isHidden) {
                this.button.classList.add("fully-hidden");
            } else {
                this.button.classList.remove("fully-hidden");
            }
        }

        click(event) {
            this.picker.hasUserSelectedDate = true;
            this.picker.goToDate(this.date);
            event.stopPropagation();
            event.preventDefault();
        }

        keyPress(event) {
            let handled = true;

            switch (event.keyCode) {
                case this.picker.keycodes.left:
                    this.picker.focusPreviousDay();
                    break;
                case this.picker.keycodes.right:
                    this.picker.focusNextDay();
                    break;
                case this.picker.keycodes.up:
                    this.picker.focusPreviousWeek();
                    break;
                case this.picker.keycodes.down:
                    this.picker.focusNextWeek();
                    break;
                case this.picker.keycodes.home:
                    this.picker.focusFirstDayOfWeek();
                    break;
                case this.picker.keycodes.end:
                    this.picker.focusLastDayOfWeek();
                    break;
                case this.picker.keycodes.pageup:
                    if (event.shiftKey) {
                        this.picker.focusPreviousYear(event);
                    } else {
                        this.picker.focusPreviousMonth(event);
                    }
                    break;
                case this.picker.keycodes.pagedown:
                    if (event.shiftKey) {
                        this.picker.focusNextYear(event);
                    } else {
                        this.picker.focusNextMonth(event);
                    }
                    break;
                default:
                    handled = false;
            }

            if (handled) {
                event.preventDefault();
                event.stopPropagation();
            }
        }
    }

    function slugify(text) {
        return String(text)
            .trim()
            .toLowerCase()
            .replace(/['"\u2018\u2019\u201C\u201D`]/g, "")
            .replace(/[\W|_]+/g, "-")
            .replace(/^-+|-+$/g, "");
    }

    const tracking = {
        init: function (scope) {
            if (!scope) {
                scope = document;
            }
            for (let key in tracking.add) {
                tracking.add[key](scope);
            }
        },

        gatherElements: function (className, scope) {
            let elements = [].slice.call(scope.querySelectorAll(`.${className}`));
            if (scope.classList && scope.classList.contains(className)) {
                elements.push(scope);
            }
            return elements;
        },

        getClickType: function (event) {
            switch (event.type) {
                case "click":
                    if (event.ctrlKey) return "ctrl click";
                    if (event.metaKey) return "command/win click";
                    if (event.shiftKey) return "shift click";
                    return "primary click";
                case "auxclick":
                    return "middle click";
                case "contextmenu":
                    return "secondary click";
            }
        },

        getNearestSectionHeader: function (element) {
            if (typeof element.closest === "function" && element.closest("nav,.ds_metadata,.ds_summary-card__header")) {
                return false;
            }

            function getPrecedingSiblings(el) {
                const siblings = [];
                if (el.parentElement) {
                    const children = [].slice.call(el.parentElement.children);
                    for (let i = 0, len = children.length; i < len && children[i] !== el; i++) {
                        siblings.push(children[i]);
                    }
                }
                return siblings;
            }

            function findMatchInSiblings(siblings, headingSelector, containerSelector) {
                siblings.reverse();
                if (!Element.prototype.matches) {
                    Element.prototype.matches = Element.prototype.msMatchesSelector || Element.prototype.webkitMatchesSelector;
                }
                for (let i = 0, len = siblings.length; i < len; i++) {
                    if (siblings[i].matches(headingSelector)) {
                        return siblings[i];
                    }
                    if (containerSelector && siblings[i].matches(containerSelector) && siblings[i].querySelector(headingSelector)) {
                        return siblings[i].querySelector(headingSelector);
                    }
                }
            }

            return findMatchInSiblings(
                getPrecedingSiblings(element),
                "h1,h2,h3,h4,h5,h6,.ds_details__summary",
                ".ds_page-header,.ds_layout__header,.ds_accordion-item__header"
            ) || (element.parentNode ? tracking.getNearestSectionHeader(element.parentNode) : false);
        },

        pushToDataLayer: function (data) {
            window.dataLayer = window.dataLayer || [];
            window.dataLayer.push(data);
        },

        add: {
            clicks: function (scope = document) {
                if (tracking.hasAddedClickTracking) return;

                scope.addEventListener("click", function (event) {
                    tracking.pushToDataLayer({ method: tracking.getClickType(event) });
                });

                scope.addEventListener("auxclick", function (event) {
                    if (event.button === 1 || event.buttons === 4) {
                        tracking.pushToDataLayer({ method: tracking.getClickType(event) });
                    }
                });

                scope.addEventListener("contextmenu", function (event) {
                    tracking.pushToDataLayer({ method: tracking.getClickType(event) });
                });

                tracking.hasAddedClickTracking = true;
            },

            canonicalUrl: function () {
                const link = document.querySelector('link[rel="canonical"]');
                if (link && link.href && !tracking.hasAddedCanonicalUrl) {
                    tracking.pushToDataLayer({ canonicalUrl: link.href });
                    tracking.hasAddedCanonicalUrl = true;
                }
            },

            prefersColorScheme: function () {
                if (!window.matchMedia) return;

                const scheme = window.matchMedia("(prefers-color-scheme: dark)").matches ? "dark" : "light";
                if (!tracking.hasAddedPrefersColorScheme) {
                    tracking.pushToDataLayer({ prefersColorScheme: scheme });
                    tracking.hasAddedPrefersColorScheme = true;
                }
            },

            version: function () {
                if (!tracking.hasAddedVersion) {
                    tracking.pushToDataLayer({ version: "v3.4.1" });
                    tracking.hasAddedVersion = true;
                }
            },

            accordions: function (scope = document) {
                tracking.gatherElements("ds_accordion", scope).forEach(function (accordion) {
                    let name = "";
                    if (accordion.dataset.name) {
                        name = accordion.dataset.name;
                    }

                    if (!accordion.classList.contains("js-initialised")) return;

                    [].slice.call(accordion.querySelectorAll("a:not(.ds_button)")).forEach(function (link) {
                        if (!link.getAttribute("data-navigation")) {
                            link.setAttribute("data-navigation", "accordion-link");
                        }
                    });

                    const openAllButton = accordion.querySelector(".js-open-all");
                    const items = [].slice.call(accordion.querySelectorAll(".ds_accordion-item"));

                    function updateOpenAllAttribute(button) {
                        if (!button) return;
                        const openCount = accordion.querySelectorAll(".ds_accordion-item--open").length;
                        const allOpen = items.length === openCount;
                        const prefix = name.length ? name + "-" : name;
                        button.setAttribute("data-accordion", `accordion-${prefix}${allOpen ? "close-all" : "open-all"}`);
                    }

                    function updateItemAttribute(item, index) {
                        const button = item.querySelector(".ds_accordion-item__button");
                        const control = item.querySelector(".ds_accordion-item__control");
                        const prefix = name.length ? name + "-" : name;
                        button.setAttribute("data-accordion", `accordion-${prefix}${control.checked ? "close" : "open"}-${index + 1}`);
                    }

                    updateOpenAllAttribute(openAllButton);

                    items.forEach(function (item, index) {
                        updateItemAttribute(item, index);
                    });

                    if (openAllButton) {
                        openAllButton.addEventListener("click", function () {
                            items.forEach(function (item, index) {
                                updateItemAttribute(item, index);
                            });
                            updateOpenAllAttribute(openAllButton);
                        });
                    }

                    items.forEach(function (item, index) {
                        const button = item.querySelector(".ds_accordion-item__button");
                        const control = item.querySelector(".ds_accordion-item__control");
                        button.addEventListener("click", function () {
                            const prefix = name.length ? name + "-" : name;
                            button.setAttribute("data-accordion", `accordion-${prefix}${control.checked ? "close" : "open"}-${index + 1}`);
                            updateOpenAllAttribute(openAllButton);
                        });
                    });
                });
            },

            asides: function (scope = document) {
                tracking.gatherElements("ds_article-aside", scope).forEach(function (aside) {
                    [].slice.call(aside.querySelectorAll("a:not(.ds_button)")).forEach(function (link, index) {
                        if (!link.getAttribute("data-navigation")) {
                            link.setAttribute("data-navigation", `link-related-${index + 1}`);
                        }
                    });
                });
            },

            autocompletes: function (scope = document) {
                function pushAutocompleteEvent(searchText, input) {
                    tracking.pushToDataLayer({
                        event: "autocomplete",
                        searchText: searchText,
                        clickText: input.dataset.autocompletetext,
                        resultsCount: parseInt(input.dataset.autocompletecount),
                        clickedResults: `result ${input.dataset.autocompleteposition} of ${input.dataset.autocompletecount}`
                    });
                    delete input.dataset.autocompletetext;
                    delete input.dataset.autocompletecount;
                    delete input.dataset.autocompleteposition;
                }

                tracking.gatherElements("ds_autocomplete", scope).forEach(function (autocomplete) {
                    const input = autocomplete.querySelector(".js-autocomplete-input");
                    const suggestionsList = document.querySelector("#" + input.getAttribute("aria-owns") + " .ds_autocomplete__suggestions-list");
                    let previousValue = input.value;

                    input.addEventListener("keyup", function (event) {
                        if (event.key === "Enter" && input.dataset.autocompletetext) {
                            pushAutocompleteEvent(previousValue, input);
                        }
                        previousValue = input.value;
                    });

                    suggestionsList.addEventListener("mousedown", function () {
                        pushAutocompleteEvent(previousValue, input);
                    });
                });
            },

            backToTop: function (scope = document) {
                tracking.gatherElements("ds_back-to-top__button", scope).forEach(function (button) {
                    button.setAttribute("data-navigation", "backtotop");
                });
            },

            breadcrumbs: function (scope = document) {
                tracking.gatherElements("ds_breadcrumbs", scope).forEach(function (breadcrumb) {
                    [].slice.call(breadcrumb.querySelectorAll(".ds_breadcrumbs__link")).forEach(function (link, index) {
                        if (!link.getAttribute("data-navigation")) {
                            link.setAttribute("data-navigation", `breadcrumb-${index + 1}`);
                        }
                    });
                });
            },

            buttons: function (scope = document) {
                [].slice.call(scope.querySelectorAll('.ds_button, input[type="button"], input[type="submit"], button')).forEach(function (button) {
                    if (!button.getAttribute("data-button")) {
                        button.setAttribute("data-button", `button-${slugify(button.textContent)}`);
                    }
                });
            },

            cards: function (scope = document) {
                tracking.gatherElements("ds_card__link--cover", scope).forEach(function (card, index) {
                    if (!card.getAttribute("data-navigation")) {
                        card.setAttribute("data-navigation", `card-${index + 1}`);
                    }
                });
            },

            categoryLists: function (scope = document) {
                tracking.gatherElements("ds_category-list", scope).forEach(function (list) {
                    [].slice.call(list.querySelectorAll(".ds_category-item__link")).forEach(function (link, index) {
                        if (!link.getAttribute("data-navigation")) {
                            link.setAttribute("data-navigation", `category-item-${index + 1}`);
                        }
                    });
                });
            },

            checkboxes: function (scope = document) {
                tracking.gatherElements("ds_checkbox__input", scope).forEach(function (checkbox) {
                    let formAttr = checkbox.getAttribute("data-form");

                    if (!formAttr && checkbox.id) {
                        formAttr = `checkbox-${checkbox.id}`;
                    } else {
                        formAttr = formAttr.replace(/-checked/g, "");
                    }

                    if (checkbox.checked) {
                        formAttr += "-checked";
                    }

                    checkbox.setAttribute("data-form", formAttr);

                    if (checkbox.id && !checkbox.getAttribute("data-value")) {
                        checkbox.setAttribute("data-value", checkbox.id);
                    }

                    const label = scope.querySelector(`[for=${checkbox.id}]`);
                    if (label && !checkbox.classList.contains("js-has-tracking-event")) {
                        label.addEventListener("click", function () {
                            checkbox.dataset.form = `checkbox-${checkbox.id}-${checkbox.checked ? "unchecked" : "checked"}`;
                        });
                        checkbox.classList.add("js-has-tracking-event");
                    }
                });
            },

            confirmationMessages: function (scope = document) {
                tracking.gatherElements("ds_confirmation-message", scope).forEach(function (message) {
                    [].slice.call(message.querySelectorAll("a:not(.ds_button)")).forEach(function (link) {
                        link.setAttribute("data-navigation", "confirmation-link");
                    });
                });
            },

            contactDetails: function (scope = document) {
                tracking.gatherElements("ds_contact-details", scope).forEach(function (details) {
                    [].slice.call(details.querySelectorAll(".ds_contact-details__social-link")).forEach(function (link) {
                        if (!link.getAttribute("data-navigation")) {
                            link.setAttribute("data-navigation", `contact-details-${slugify(link.textContent)}`);
                        }
                    });

                    [].slice.call(details.querySelectorAll('a[href^="mailto"]')).forEach(function (link) {
                        if (!link.getAttribute("data-navigation")) {
                            link.setAttribute("data-navigation", "contact-details-email");
                        }
                    });
                });
            },

            contentNavs: function (scope = document) {
                tracking.gatherElements("ds_contents-nav", scope).forEach(function (nav) {
                    [].slice.call(nav.querySelectorAll(".ds_contents-nav__link")).forEach(function (link, index) {
                        if (!link.getAttribute("data-navigation")) {
                            link.setAttribute("data-navigation", `contentsnav-${index + 1}`);
                        }
                    });
                });
            },

            details: function (scope = document) {
                tracking.gatherElements("ds_details", scope).forEach(function (detail) {
                    const summary = detail.querySelector(".ds_details__summary");
                    summary.setAttribute("data-accordion", "detail-" + (detail.open ? "close" : "open"));

                    summary.addEventListener("click", function () {
                        summary.setAttribute("data-accordion", "detail-" + (detail.open ? "open" : "close"));
                    });

                    [].slice.call(detail.querySelectorAll("a:not(.ds_button)")).forEach(function (link) {
                        if (!link.getAttribute("data-navigation")) {
                            link.setAttribute("data-navigation", "details-link");
                        }
                    });
                });
            },

            errorMessages: function (scope = document) {
                tracking.gatherElements("ds_question__error-message", scope).forEach(function (errorMsg, index) {
                    if (typeof errorMsg.closest !== "function" || !errorMsg.closest(".ds_question")) return;

                    const question = errorMsg.closest(".ds_question");
                    const inputElement = question.querySelector(".js-validation-group, .ds_input, .ds_select, .ds_checkbox__input, .ds_radio__input");
                    let identifier = index + 1;

                    if (inputElement) {
                        if (inputElement.classList.contains("js-validation-group")) {
                            function isUnique(value, idx, arr) {
                                return arr.indexOf(value) === idx;
                            }
                            identifier = [].slice.call(inputElement.querySelectorAll(".ds_input, .ds_select, .ds_checkbox__input, .ds_radio__input"))
                                .map(function (el) { return el.type === "radio" ? el.name : el.id; })
                                .filter(isUnique)
                                .join("-");
                        } else {
                            identifier = inputElement.type === "radio" ? inputElement.name : inputElement.id;
                        }
                    }

                    if (!errorMsg.getAttribute("data-form")) {
                        errorMsg.setAttribute("data-form", `error-${identifier}`);
                    }
                });
            },

            errorSummaries: function (scope = document) {
                tracking.gatherElements("ds_error-summary", scope).forEach(function (summary) {
                    [].slice.call(summary.querySelectorAll(".ds_error-summary__list a")).forEach(function (link) {
                        if (!link.getAttribute("data-form") && link.href) {
                            link.setAttribute("data-form", `error-${link.href.substring(link.href.lastIndexOf("#") + 1)}`);
                        }
                    });
                });
            },

            externalLinks: function (scope = document) {
                [].slice.call(scope.querySelectorAll("a")).filter(function (link) {
                    let host = window.location.hostname;
                    if (window.location.port) {
                        host += ":" + window.location.port;
                    }
                    return !new RegExp("/" + host + "/?|^tel:|^mailto:|^/").test(link.href);
                }).forEach(function (link) {
                    link.setAttribute("data-navigation", "link-external");
                });
            },

            hideThisPage: function (scope = document) {
                tracking.gatherElements("ds_hide-page", scope).forEach(function (hidePage) {
                    [].slice.call(hidePage.querySelectorAll(".ds_hide-page__button")).forEach(function (button) {
                        button.setAttribute("data-navigation", "hide-this-page");
                        document.addEventListener("keyup", function (event) {
                            if (event.key === "Escape" || event.keyCode === 27) {
                                tracking.pushToDataLayer({ event: "hide-this-page-keyboard" });
                            }
                        });
                    });
                });
            },

            insetTexts: function (scope = document) {
                tracking.gatherElements("ds_inset-text", scope).forEach(function (inset) {
                    [].slice.call(inset.querySelectorAll(".ds_inset-text__text a:not(.ds_button)")).forEach(function (link) {
                        if (!link.getAttribute("data-navigation")) {
                            link.setAttribute("data-navigation", "inset-link");
                        }
                    });
                });
            },

            links: function () {
                [].slice.call(document.querySelectorAll("a")).forEach(function (link) {
                    const header = tracking.getNearestSectionHeader(link);
                    if (header && !link.getAttribute("data-section")) {
                        link.setAttribute("data-section", header.textContent.trim());
                    }
                });
            },

            metadataItems: function (scope = document) {
                tracking.gatherElements("ds_metadata__item", scope).forEach(function (item, itemIndex) {
                    const keyElement = item.querySelector(".ds_metadata__key");
                    let keyText = keyElement ? keyElement.textContent.trim() : `metadata-${itemIndex}`;

                    [].slice.call(item.querySelectorAll(".ds_metadata__value a")).forEach(function (link, linkIndex) {
                        if (!link.getAttribute("data-navigation")) {
                            link.setAttribute("data-navigation", `${slugify(keyText)}-${linkIndex + 1}`);
                        }
                    });
                });
            },

            notifications: function (scope = document) {
                tracking.gatherElements("ds_notification", scope).forEach(function (notification, index) {
                    const notificationId = notification.id || index + 1;

                    [].slice.call(notification.querySelectorAll("a:not(.ds_button)")).forEach(function (link) {
                        if (!link.getAttribute("data-banner")) {
                            link.setAttribute("data-banner", `banner-${notificationId}-link`);
                        }
                    });

                    [].slice.call(notification.querySelectorAll(".ds_button:not(.ds_notification__close)")).forEach(function (button) {
                        if (!button.getAttribute("data-banner")) {
                            button.setAttribute("data-banner", `banner-${notificationId}-${slugify(button.textContent)}`);
                        }
                    });

                    const closeButton = notification.querySelector(".ds_notification__close");
                    if (closeButton && !closeButton.getAttribute("data-banner")) {
                        closeButton.setAttribute("data-banner", `banner-${notificationId}-close`);
                    }
                });
            },

            pagination: function (scope = document) {
                tracking.gatherElements("ds_pagination", scope).forEach(function (pagination) {
                    const loadMoreButton = pagination.querySelector(".ds_pagination__load-more button");
                    if (loadMoreButton && !loadMoreButton.getAttribute("data-search")) {
                        loadMoreButton.setAttribute("data-search", "pagination-more");
                    }

                    [].slice.call(pagination.querySelectorAll("a.ds_pagination__link")).forEach(function (link) {
                        if (!link.getAttribute("data-search")) {
                            link.setAttribute("data-search", `pagination-${slugify(link.textContent)}`);
                        }
                    });
                });
            },

            phaseBanners: function (scope = document) {
                tracking.gatherElements("ds_phase-banner", scope).forEach(function (banner) {
                    const tagElement = banner.querySelector(".ds_tag");
                    const tagText = tagElement ? tagElement.textContent.trim() : "phase";

                    [].slice.call(banner.querySelectorAll("a")).forEach(function (link) {
                        if (!link.getAttribute("data-banner")) {
                            link.setAttribute("data-banner", `banner-${slugify(tagText)}-link`);
                        }
                    });
                });
            },

            radios: function (scope = document) {
                tracking.gatherElements("ds_radio__input", scope).forEach(function (radio) {
                    if (!radio.getAttribute("data-form") && radio.name && radio.id) {
                        radio.setAttribute("data-form", `radio-${radio.name}-${radio.id}`);
                    }
                    if (radio.id && !radio.getAttribute("data-value")) {
                        radio.setAttribute("data-value", radio.id);
                    }
                });
            },

            searchFacets: function (scope = document) {
                tracking.gatherElements("ds_facet__button", scope).forEach(function (button) {
                    button.setAttribute("data-button", `button-filter-${button.dataset.slug}-remove`);
                });
            },

            searchResults: function (scope = document) {
                tracking.gatherElements("ds_search-results", scope).forEach(function (container) {
                    const resultsList = container.querySelector(".ds_search-results__list");
                    if (!resultsList) return;

                    const allResults = [].slice.call(container.querySelectorAll(".ds_search-result"));
                    const promotedResults = [].slice.call(container.querySelectorAll(".ds_search-result--promoted"));
                    let startIndex = 1;

                    if (resultsList.getAttribute("start")) {
                        startIndex = +resultsList.getAttribute("start");
                    }

                    allResults.forEach(function (result, index) {
                        const resultLink = result.querySelector(".ds_search-result__link");
                        const mediaLink = result.querySelector(".ds_search-result__media-link");
                        const contextLink = result.querySelector(".ds_search-result__context a");

                        if (result.classList.contains("ds_search-result--promoted")) {
                            resultLink.setAttribute("data-search", `search-promoted-${index + 1}/${promotedResults.length}`);
                        } else {
                            const total = resultsList.getAttribute("data-total") || null;
                            let resultAttr = "search-result-" + (startIndex + index - promotedResults.length);
                            let imageAttr = "search-image-" + (startIndex + index - promotedResults.length);
                            let parentAttr = "search-parent-link-" + (startIndex + index - promotedResults.length);

                            if (total) {
                                resultAttr += `/${total}`;
                                parentAttr += `/${total}`;
                            }

                            resultLink.setAttribute("data-search", resultAttr);
                            if (mediaLink) {
                                mediaLink.setAttribute("data-search", imageAttr);
                            }
                            if (contextLink) {
                                contextLink.setAttribute("data-search", parentAttr);
                            }
                        }
                    });
                });
            },

            searchSuggestions: function (scope = document) {
                tracking.gatherElements("ds_search-suggestions", scope).forEach(function (container) {
                    const links = [].slice.call(container.querySelectorAll(".ds_search-suggestions a"));
                    links.forEach(function (link, index) {
                        link.setAttribute("data-search", `suggestion-result-${index + 1}/${links.length}`);
                    });
                });
            },

            searchRelated: function (scope = document) {
                tracking.gatherElements("ds_search-results__related", scope).forEach(function (container) {
                    const links = [].slice.call(container.querySelectorAll(".ds_search-results__related a"));
                    links.forEach(function (link, index) {
                        link.setAttribute("data-search", `search-related-${index + 1}/${links.length}`);
                    });
                });
            },

            selects: function (scope = document) {
                tracking.gatherElements("ds_select", scope).forEach(function (select) {
                    if (!select.getAttribute("data-form") && select.id) {
                        select.setAttribute("data-form", `select-${select.id}`);
                    }

                    [].slice.call(select.querySelectorAll("option")).forEach(function (option) {
                        let optionValue = "null";
                        if (option.value) {
                            optionValue = slugify(option.value);
                        }
                        option.setAttribute("data-form", `${select.getAttribute("data-form")}-${optionValue}`);
                    });

                    if (!select.classList.contains("js-has-tracking-event")) {
                        select.addEventListener("change", function (event) {
                            tracking.pushToDataLayer({
                                event: event.target.querySelector(":checked").dataset.form
                            });
                        });
                        select.classList.add("js-has-tracking-event");
                    }
                });
            },

            sequentialNavs: function (scope = document) {
                tracking.gatherElements("ds_sequential-nav", scope).forEach(function (nav) {
                    const prevButton = nav.querySelector(".ds_sequential-nav__item--prev > .ds_sequential-nav__button ");
                    const nextButton = nav.querySelector(".ds_sequential-nav__item--next > .ds_sequential-nav__button ");

                    if (prevButton && !prevButton.getAttribute("data-navigation")) {
                        prevButton.setAttribute("data-navigation", "sequential-previous");
                    }
                    if (nextButton && !nextButton.getAttribute("data-navigation")) {
                        nextButton.setAttribute("data-navigation", "sequential-next");
                    }
                });
            },

            sideNavs: function (scope = document) {
                tracking.gatherElements("ds_side-navigation", scope).forEach(function (sideNav) {
                    const list = sideNav.querySelector(".ds_side-navigation__list");
                    const toggleButton = sideNav.querySelector(".js-side-navigation-button");
                    const toggleCheckbox = sideNav.querySelector(".js-toggle-side-navigation");

                    function updateToggleAttribute() {
                        toggleButton.setAttribute("data-navigation", "navigation-" + (toggleCheckbox.checked ? "close" : "open"));
                    }

                    function assignNavAttributes(listElement, prefix = "") {
                        [].slice.call(listElement.children).forEach(function (child, index) {
                            [].slice.call(child.children).forEach(function (el) {
                                if (el.classList.contains("ds_side-navigation__list")) {
                                    assignNavAttributes(el, `${prefix}-${index + 1}`);
                                } else {
                                    el.setAttribute("data-navigation", `sidenav${prefix}-${index + 1}`);
                                }
                            });
                        });
                    }

                    assignNavAttributes(list);

                    if (toggleButton) {
                        updateToggleAttribute();
                        toggleButton.addEventListener("click", function () {
                            updateToggleAttribute();
                        });
                    }
                });
            },

            siteBranding: function (scope = document) {
                tracking.gatherElements("ds_site-branding", scope).forEach(function (branding) {
                    const logo = branding.querySelector(".ds_site-branding__logo");
                    if (logo && !logo.getAttribute("data-header")) {
                        logo.setAttribute("data-header", "header-logo");
                    }

                    const title = branding.querySelector(".ds_site-branding__title");
                    if (title && !title.getAttribute("data-header")) {
                        title.setAttribute("data-header", "header-title");
                    }
                });
            },

            siteFooter: function (scope = document) {
                tracking.gatherElements("ds_site-footer", scope).forEach(function (footer) {
                    [].slice.call(footer.querySelectorAll(".ds_site-footer__org-link")).forEach(function (link) {
                        if (!link.getAttribute("data-footer")) {
                            link.setAttribute("data-footer", "footer-logo");
                        }
                    });

                    [].slice.call(footer.querySelectorAll(".ds_site-footer__copyright a")).forEach(function (link) {
                        if (!link.getAttribute("data-footer")) {
                            link.setAttribute("data-footer", "footer-copyright");
                        }
                    });

                    [].slice.call(footer.querySelectorAll(".ds_site-items__item a:not(.ds_button)")).forEach(function (link, index) {
                        if (!link.getAttribute("data-footer")) {
                            link.setAttribute("data-footer", `footer-link-${index + 1}`);
                        }
                    });
                });
            },

            siteNavigation: function (scope = document) {
                tracking.gatherElements("ds_site-navigation", scope).forEach(function (nav) {
                    [].slice.call(nav.querySelectorAll(".ds_site-navigation__link")).forEach(function (link, index) {
                        if (!link.getAttribute("data-device")) {
                            if (typeof link.closest === "function" && link.closest(".ds_site-navigation--mobile")) {
                                link.setAttribute("data-device", "mobile");
                            } else {
                                link.setAttribute("data-device", "desktop");
                            }
                        }
                        if (!link.getAttribute("data-header")) {
                            link.setAttribute("data-header", `header-link-${index + 1}`);
                        }
                    });
                });

                tracking.gatherElements("ds_site-navigation--mobile", scope).forEach(function (mobileNav) {
                    const menuToggle = mobileNav.parentNode.querySelector(".js-toggle-menu");
                    if (menuToggle) {
                        menuToggle.setAttribute("data-header", "header-menu-toggle");
                    }
                });
            },

            skipLinks: function (scope = document) {
                [].slice.call(scope.querySelectorAll(".ds_skip-links__link")).forEach(function (link, index) {
                    if (!link.getAttribute("data-navigation")) {
                        link.setAttribute("data-navigation", `skip-link-${index + 1}`);
                    }
                });
            },

            stepNavigation: function (scope = document) {
                tracking.gatherElements("ds_step-navigation", scope).forEach(function (stepNav) {
                    [].slice.call(stepNav.querySelectorAll(".ds_step-navigation__title-link")).forEach(function (link) {
                        link.setAttribute("data-navigation", "partof-sidebar");
                    });
                });

                tracking.gatherElements("ds_step-navigation-top", scope).forEach(function (stepNavTop) {
                    [].slice.call(stepNavTop.querySelectorAll("a")).forEach(function (link) {
                        link.setAttribute("data-navigation", "partof-header");
                    });
                });
            },

            summaryCard: function (scope = document) {
                tracking.gatherElements("ds_summary-card", scope).forEach(function (card, cardIndex) {
                    [].slice.call(card.querySelectorAll(".ds_summary-card__actions-list")).forEach(function (actionsList) {
                        [].slice.call(actionsList.querySelectorAll("button")).forEach(function (button) {
                            button.setAttribute("data-button", `button-${slugify(button.textContent)}-${cardIndex + 1}`);
                        });

                        [].slice.call(actionsList.querySelectorAll("a")).forEach(function (link) {
                            link.setAttribute("data-navigation", `navigation-${slugify(link.textContent)}-${cardIndex + 1}`);
                        });
                    });
                });
            },

            summaryList: function (scope = document) {
                tracking.gatherElements("ds_summary-list__actions", scope).forEach(function (actions) {
                    [].slice.call(actions.querySelectorAll("button, a")).forEach(function (element) {
                        const dataType = element.tagName === "BUTTON" ? "button" : "navigation";
                        const keySuffix = "-" + slugify(element.closest(".ds_summary-list__item").querySelector(".ds_summary-list__key").textContent);
                        element.setAttribute(`data-${dataType}`, `${dataType}-${slugify(element.textContent)}${keySuffix}`);
                    });
                });
            },

            tabs: function (scope = document) {
                const tabGroups = tracking.gatherElements("ds_tabs", scope);
                let groupIndex = 1;

                tabGroups.forEach(function (tabGroup) {
                    [].slice.call(tabGroup.querySelectorAll(".ds_tabs__tab-link")).forEach(function (link, linkIndex) {
                        if (!link.getAttribute("data-navigation")) {
                            link.setAttribute("data-navigation", `tab-link-${groupIndex}-${linkIndex + 1}`);
                        }
                    });
                    groupIndex++;
                });
            },

            taskList: function (scope = document) {
                tracking.gatherElements("ds_task-list__task-link", scope).forEach(function (link) {
                    if (!link.getAttribute("data-navigation")) {
                        link.setAttribute("data-navigation", "tasklist");
                    }
                });

                tracking.gatherElements("js-task-list-skip-link", scope).forEach(function (link) {
                    if (!link.getAttribute("data-navigation")) {
                        link.setAttribute("data-navigation", "tasklist-skip");
                    }
                });
            },

            textInputs: function (scope = document) {
                [].slice.call(scope.querySelectorAll("input.ds_input")).forEach(function (input) {
                    if (!input.getAttribute("data-form") && input.id) {
                        input.setAttribute("data-form", `${input.type}input-${input.id}`);
                    }
                });
            },

            textareas: function (scope = document) {
                [].slice.call(scope.querySelectorAll("textarea.ds_input")).forEach(function (textarea) {
                    if (!textarea.getAttribute("data-form") && textarea.id) {
                        textarea.setAttribute("data-form", `textarea-${textarea.id}`);
                    }
                });
            },

            warningTexts: function (scope = document) {
                tracking.gatherElements("ds_warning-text", scope).forEach(function (warning) {
                    [].slice.call(warning.querySelectorAll(".ds_warning-text a:not(.ds_button)")).forEach(function (link) {
                        link.setAttribute("data-navigation", "warning-link");
                    });
                });
            }
        }
    };

    const trackingRef = tracking;

    function getWebfilePath(suffix = "") {
        const pathElement = document.getElementById("br-webfile-path");
        return pathElement ? pathElement.value + suffix : suffix;
    }

    window.DS = window.DS || {};
    window.DS.components = window.DS.components || {
        DatePicker: class {
            constructor(element, options = {}) {
                if (!element) return;

                this.datePickerParent = element;
                this.options = options;
                this.inputElement = this.datePickerParent.querySelector("input");
                this.isMultipleInput = element.classList.contains("ds_datepicker--multiple");
                this.dateInput = element.querySelector(".js-datepicker-date");
                this.monthInput = element.querySelector(".js-datepicker-month");
                this.yearInput = element.querySelector(".js-datepicker-year");
                this.dayLabels = ["Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday"];
                this.monthLabels = ["January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December"];
                this.currentDate = new Date();
                this.currentDate.setHours(0, 0, 0, 0);
                this.calendarDays = [];
                this.disabledDates = [];
                this.hasUserSelectedDate = false;
                this.keycodes = {
                    tab: 9,
                    pageup: 33,
                    pagedown: 34,
                    end: 35,
                    home: 36,
                    left: 37,
                    up: 38,
                    right: 39,
                    down: 40
                };
            }

            init() {
                if (!this.inputElement || this.datePickerParent.classList.contains("js-initialised")) {
                    return;
                }

                this.setOptions();

                this.icons = {
                    calendar_today: SVG_CALENDAR_TODAY,
                    chevron_left: SVG_CHEVRON_LEFT,
                    chevron_right: SVG_CHEVRON_RIGHT,
                    double_chevron_left: SVG_DOUBLE_CHEVRON_LEFT,
                    double_chevron_right: SVG_DOUBLE_CHEVRON_RIGHT
                };

                this.calendarToggleButton = this.datePickerParent.querySelector(".js-calendar-toggle-btn");
                this.calendarContainer = this.datePickerParent.querySelector("#calendar-container");
                this.useInlineMode = !!(this.calendarToggleButton && this.calendarContainer);

                if (this.useInlineMode) {
                    this.calendarButtonElement = document.createElement("button");
                    this.calendarButtonElement.type = "button";
                    this.calendarButtonElement.style.display = "none";
                    this.calendarButtonElement.innerHTML = '<span class="visually-hidden">Choose date</span>';
                    this.datePickerParent.appendChild(this.calendarButtonElement);
                } else {
                    const tempWrapper = document.createElement("div");
                    tempWrapper.innerHTML = this.buttonTemplate();
                    this.calendarButtonElement = tempWrapper.firstChild;
                    this.calendarButtonElement.setAttribute("data-button", `datepicker-${this.inputElement.id}-toggle`);

                    if (this.isMultipleInput) {
                        const wrapper = document.createElement("div");
                        wrapper.className = "govuk-date-input__item ds_datepicker__button-wrapper";
                        wrapper.appendChild(this.calendarButtonElement);

                        const calContainer = this.datePickerParent.querySelector("#calendar-container");
                        if (calContainer) {
                            this.datePickerParent.insertBefore(wrapper, calContainer);
                        } else {
                            this.datePickerParent.appendChild(wrapper);
                        }
                    } else {
                        this.inputElement.parentNode.appendChild(this.calendarButtonElement);
                        this.inputElement.parentNode.classList.add("ds_input__wrapper--has-icon");
                    }
                }

                window.DS = window.DS || {};
                window.DS.elementIdModifier = window.DS.elementIdModifier || 0;
                window.DS.elementIdModifier += 1;

                const dialogContainer = document.createElement("div");
                dialogContainer.id = `datepicker-ds${window.DS.elementIdModifier}`;
                dialogContainer.setAttribute("class", "ds_datepicker__dialog  datepickerDialog");
                dialogContainer.setAttribute("role", "dialog");
                dialogContainer.setAttribute("aria-modal", "true");
                dialogContainer.innerHTML = this.dialogTemplate(dialogContainer.id);

                this.calendarButtonElement.setAttribute("aria-controls", dialogContainer.id);
                this.calendarButtonElement.setAttribute("aria-expanded", false);
                this.dialogElement = dialogContainer;

                if (this.useInlineMode) {
                    this.calendarContainer.appendChild(dialogContainer);
                    this.dialogElement.classList.add("ds_datepicker__dialog--open");
                    this.dialogElement.classList.add("ds_datepicker__dialog--inline");
                } else {
                    this.datePickerParent.appendChild(dialogContainer);
                }

                this.dialogTitleNode = this.dialogElement.querySelector(".js-datepicker-month-year");
                this.setMinAndMaxDatesOnCalendar();

                const tableBody = this.datePickerParent.querySelector("tbody");
                let dayIndex = 0;

                for (let rowIndex = 0; rowIndex < 6; rowIndex++) {
                    const row = tableBody.insertRow(rowIndex);

                    for (let colIndex = 0; colIndex < 7; colIndex++) {
                        const cell = document.createElement("td");
                        const dayButton = document.createElement("button");
                        dayButton.type = "button";
                        dayButton.dataset.form = "date-select";
                        cell.appendChild(dayButton);
                        row.appendChild(cell);

                        const calendarDay = new CalendarDay(dayButton, dayIndex, rowIndex, colIndex, this);
                        calendarDay.init();
                        this.calendarDays.push(calendarDay);
                        dayIndex++;
                    }
                }

                this.prevMonthButton = this.dialogElement.querySelector(".js-datepicker-prev-month");
                this.prevYearButton = this.dialogElement.querySelector(".js-datepicker-prev-year");
                this.nextMonthButton = this.dialogElement.querySelector(".js-datepicker-next-month");
                this.nextYearButton = this.dialogElement.querySelector(".js-datepicker-next-year");

                this.prevMonthButton.addEventListener("click", (event) => this.focusPreviousMonth(event, false));
                this.prevYearButton.addEventListener("click", (event) => this.focusPreviousYear(event, false));
                this.nextMonthButton.addEventListener("click", (event) => this.focusNextMonth(event, false));
                this.nextYearButton.addEventListener("click", (event) => this.focusNextYear(event, false));

                [this.inputElement, this.dateInput, this.monthInput, this.yearInput].forEach((input) => {
                    if (input) {
                        input.addEventListener("blur", () => {
                            this.calendarButtonElement.querySelector("span").textContent = "Choose date";
                        });
                    }
                });

                this.cancelButton = this.dialogElement.querySelector(".js-datepicker-cancel");
                this.okButton = this.dialogElement.querySelector(".js-datepicker-ok");

                if (this.useInlineMode) {
                    this.cancelButton.addEventListener("click", (event) => {
                        event.preventDefault();
                        this.calendarContainer.style.display = "none";
                        this.calendarToggleButton.setAttribute("aria-expanded", "false");
                    });

                    this.okButton.addEventListener("click", () => {
                        this.selectDate(this.currentDate);
                        this.calendarContainer.style.display = "none";
                        this.calendarToggleButton.setAttribute("aria-expanded", "false");
                    });
                } else {
                    this.cancelButton.addEventListener("click", (event) => {
                        event.preventDefault();
                        this.closeDialog(event);
                    });

                    this.okButton.addEventListener("click", () => this.selectDate(this.currentDate));
                }

                const allButtons = this.dialogElement.querySelectorAll('button:not([disabled="true"])');
                this.firstButtonInDialog = allButtons[0];
                this.lastButtonInDialog = allButtons[allButtons.length - 1];
                this.firstButtonInDialog.addEventListener("keydown", (event) => this.firstButtonKeyup(event));
                this.lastButtonInDialog.addEventListener("keydown", (event) => this.lastButtonKeyup(event));

                if (this.useInlineMode) {
                    this.calendarToggleButton.addEventListener("click", (evt) => {
                        evt.preventDefault();
                        const isVisible = this.calendarContainer.style.display !== "none";

                        if (isVisible) {
                            this.calendarContainer.style.display = "none";
                            this.calendarToggleButton.setAttribute("aria-expanded", "false");
                        } else {
                            this.setMinAndMaxDatesOnCalendar();
                            this.hasUserSelectedDate = false;

                            if (this.isMultipleInput) {
                                const day = this.dateInput.value.trim();
                                const month = this.monthInput.value.trim();
                                const year = this.yearInput.value.trim();

                                if (day && month && year) {
                                    const parsed = new Date(parseInt(year, 10), parseInt(month, 10) - 1, parseInt(day, 10));
                                    if (!isNaN(parsed.getTime())) {
                                        this.inputDate = parsed;
                                        this.currentDate = new Date(parsed);
                                        this.hasUserSelectedDate = true;
                                    }
                                } else {
                                    this.inputDate = null;
                                }
                            }

                            this.updateCalendar();
                            this.setCurrentDate(false);
                            this.calendarContainer.style.display = "";
                            this.calendarToggleButton.setAttribute("aria-expanded", "true");
                        }
                    });
                } else {
                    this.calendarButtonElement.addEventListener("click", (event) => this.toggleDialog(event));
                }

                document.body.addEventListener("mouseup", (event) => this.backgroundClick(event));

                this.updateCalendar();
                this.datePickerParent.classList.add("js-initialised");
            }

            addMonths(date, months) {
                const originalDay = date.getDate();
                date.setMonth(date.getMonth() + +months);
                if (date.getDate() !== originalDay) {
                    date.setDate(0);
                }
                return date;
            }

            buttonTemplate() {
                return `<button type="button" class="ds_button  ds_button--icon-only  ds_datepicker__button  ds_no-margin  js-calendar-button" aria-expanded="false">
            <span class="visually-hidden">Choose date</span>
            ${this.icons.calendar_today}
        </button>
        `;
            }

            dialogTemplate(dialogId) {
                return `<div class="ds_datepicker__dialog__header">
        <div class="ds_datepicker__dialog__navbuttons">
            <button type="button" class="ds_button  ds_button--icon-only  js-datepicker-prev-year" aria-label="previous year" data-button="button-datepicker-prevyear">
                <span class="visually-hidden">Previous year</span>
                ${this.icons.double_chevron_left}
            </button>

            <button type="button" class="ds_button  ds_button--icon-only  js-datepicker-prev-month" aria-label="previous month" data-button="button-datepicker-prevmonth">
                <span class="visually-hidden">Previous month</span>
                ${this.icons.chevron_left}
            </button>
        </div>

        <h2 class="ds_datepicker__dialog__title  js-datepicker-month-year" aria-live="polite">June 2020</h2>

        <div class="ds_datepicker__dialog__navbuttons">
            <button type="button" class="ds_button  ds_button--icon-only  js-datepicker-next-month" aria-label="next month" data-button="button-datepicker-nextmonth">
                <span class="visually-hidden">Next month</span>
                ${this.icons.chevron_right}
            </button>

            <button type="button" class="ds_button  ds_button--icon-only  js-datepicker-next-year" aria-label="next year" data-button="button-datepicker-nextyear">
                <span class="visually-hidden">Next year</span>
                ${this.icons.double_chevron_right}
            </button>
        </div>
      </div>

      <table class="ds_datepicker__dialog__table  js-datepicker-grid" role="grid">
      <thead>
          <tr>
          <th scope="col">
            <span aria-hidden="true">Sun</span>
            <span class="visually-hidden">Sunday</span>
          </th>
          <th scope="col">
            <span aria-hidden="true">Mon</span>
            <span class="visually-hidden">Monday</span>
          </th>
          <th scope="col">
            <span aria-hidden="true">Tue</span>
            <span class="visually-hidden">Tuesday</span>
          </th>
          <th scope="col">
            <span aria-hidden="true">Wed</span>
            <span class="visually-hidden">Wednesday</span>
          </th>
          <th scope="col">
            <span aria-hidden="true">Thu</span>
            <span class="visually-hidden">Thursday</span>
          </th>
          <th scope="col">
            <span aria-hidden="true">Fri</span>
            <span class="visually-hidden">Friday</span>
          </th>
          <th scope="col">
            <span aria-hidden="true">Sat</span>
            <span class="visually-hidden">Saturday</span>
          </th>
          </tr>
      </thead>

      <tbody></tbody>
      </table>

      <div class="ds_datepicker__dialog__buttongroup">
      <button type="button" class="ds_button  ds_button--small  js-datepicker-ok" value="ok" data-button="button-datepicker-ok">Select</button>
      <button type="button" class="ds_button  ds_button--small  ds_button--cancel  js-datepicker-cancel" value="cancel" data-button="button-datepicker-cancel">Close</button>
      </div>`;
            }

            leadingZeroes(value, length = 2) {
                let result = value.toString();
                while (result.length < length) {
                    result = "0" + result;
                }
                return result;
            }

            backgroundClick(event) {
                if (this.useInlineMode) return;

                if (this.isOpen() &&
                    !this.dialogElement.contains(event.target) &&
                    !this.inputElement.contains(event.target) &&
                    !this.calendarButtonElement.contains(event.target)) {
                    event.preventDefault();
                    this.closeDialog();
                }
            }

            closeDialog() {
                this.dialogElement.classList.remove("ds_datepicker__dialog--open");
                this.calendarButtonElement.setAttribute("aria-expanded", false);
                this.calendarButtonElement.focus();
            }

            firstButtonKeyup(event) {
                if (event.keyCode === this.keycodes.tab && event.shiftKey) {
                    this.lastButtonInDialog.focus();
                    event.preventDefault();
                }
            }

            focusNextDay(date = new Date(this.currentDate)) {
                date.setDate(date.getDate() + 1);
                this.goToDate(date);
            }

            focusPreviousDay(date = new Date(this.currentDate)) {
                date.setDate(date.getDate() - 1);
                this.goToDate(date);
            }

            focusNextWeek(date = new Date(this.currentDate)) {
                date.setDate(date.getDate() + 7);
                this.goToDate(date);
            }

            focusPreviousWeek(date = new Date(this.currentDate)) {
                date.setDate(date.getDate() - 7);
                this.goToDate(date);
            }

            focusFirstDayOfWeek() {
                const date = new Date(this.currentDate);
                date.setDate(date.getDate() - date.getDay());
                this.goToDate(date);
            }

            focusLastDayOfWeek() {
                const date = new Date(this.currentDate);
                date.setDate(date.getDate() - date.getDay() + 6);
                this.goToDate(date);
            }

            focusNextMonth(event, shouldFocus = true) {
                event.preventDefault();
                const date = new Date(this.currentDate);
                this.addMonths(date, 1);
                this.goToDate(date, shouldFocus);
            }

            focusPreviousMonth(event, shouldFocus = true) {
                event.preventDefault();
                const date = new Date(this.currentDate);
                this.addMonths(date, -1);
                this.goToDate(date, shouldFocus);
            }

            focusNextYear(event, shouldFocus = true) {
                event.preventDefault();
                const date = new Date(this.currentDate);
                date.setFullYear(date.getFullYear() + 1);
                this.goToDate(date, shouldFocus);
            }

            focusPreviousYear(event, shouldFocus = true) {
                event.preventDefault();
                const date = new Date(this.currentDate);
                date.setFullYear(date.getFullYear() - 1);
                this.goToDate(date, shouldFocus);
            }

            formattedDateFromString(dateString, fallback = new Date()) {
                let parsedDate = null;
                const parts = dateString.split("/");

                if (dateString.match(/\d{1,4}\/\d{1,2}\/\d{1,4}/)) {
                    switch (this.datePickerParent.dataset.dateformat) {
                        case "YMD":
                            parsedDate = new Date(`${parts[1]}/${parts[2]}/${parts[0]}`);
                            break;
                        case "MDY":
                            parsedDate = new Date(`${parts[0]}/${parts[1]}/${parts[2]}`);
                            break;
                        default:
                            parsedDate = new Date(`${parts[1]}/${parts[0]}/${parts[2]}`);
                    }
                }

                return (parsedDate instanceof Date && !isNaN(parsedDate)) ? parsedDate : fallback;
            }

            formattedDateHuman(date) {
                return `${this.dayLabels[date.getDay()]} ${date.getDate()} ${this.monthLabels[date.getMonth()]} ${date.getFullYear()}`;
            }

            goToDate(date, shouldFocus) {
                const previousDate = this.currentDate;
                this.currentDate = date;

                if (previousDate.getMonth() !== this.currentDate.getMonth() || previousDate.getFullYear() !== this.currentDate.getFullYear()) {
                    this.updateCalendar();
                }

                this.setCurrentDate(shouldFocus);
            }

            isDisabledDate(date) {
                let disabled = false;

                if (this.minDate && this.minDate > date) {
                    disabled = true;
                }

                if (this.maxDate && this.maxDate < date) {
                    disabled = true;
                }

                for (const disabledDate of this.disabledDates) {
                    if (date.toDateString() === disabledDate.toDateString()) {
                        disabled = true;
                    }
                }

                return disabled;
            }

            isOpen() {
                return this.dialogElement.classList.contains("ds_datepicker__dialog--open");
            }

            lastButtonKeyup(event) {
                if (event.keyCode === this.keycodes.tab && !event.shiftKey) {
                    this.firstButtonInDialog.focus();
                    event.preventDefault();
                }
            }

            openDialog() {
                this.dialogElement.classList.add("ds_datepicker__dialog--open");
                this.calendarButtonElement.setAttribute("aria-expanded", true);

                let offsetLeft;
                let dateValue;

                if (this.isMultipleInput) {
                    offsetLeft = this.calendarButtonElement.offsetLeft + this.calendarButtonElement.offsetWidth + 16;
                    dateValue = `${this.dateInput.value}/${this.monthInput.value}/${this.yearInput.value}`;
                } else {
                    offsetLeft = this.inputElement.offsetWidth + 16;
                    dateValue = this.inputElement.value;
                }

                const gridUnits = Math.ceil(offsetLeft / 8);

                this.dialogElement.classList.forEach(function (cls) {
                    if (cls.match(/ds_!_off-l-/)) {
                        this.dialogElement.classList.remove(cls);
                    }
                }.bind(this));

                this.dialogElement.classList.add(`ds_!_off-l-${gridUnits}`);

                if (dateValue.match(/^\d{1,2}\/\d{1,2}\/\d{4}$/)) {
                    this.inputDate = this.formattedDateFromString(dateValue);
                    this.currentDate = this.inputDate;
                    this.hasUserSelectedDate = true;
                } else {
                    this.inputDate = null;
                    this.hasUserSelectedDate = false;
                }

                this.updateCalendar();
                this.setCurrentDate();
            }

            selectDate(date) {
                if (this.isDisabledDate(date)) {
                    return false;
                }

                this.calendarButtonElement.querySelector("span").textContent = "Choose date";
                this.setDate(date);
                this.hasUserSelectedDate = true;

                const changeEvent = document.createEvent("Event");
                changeEvent.initEvent("change", true, true);
                this.inputElement.dispatchEvent(changeEvent);

                if (this.dateSelectCallback) {
                    this.dateSelectCallback(date);
                }

                if (!this.useInlineMode) {
                    this.closeDialog();
                }
            }

            setCurrentDate(shouldFocus = true) {
                const focusDate = this.currentDate;
                const visibleDays = this.calendarDays.filter(function (day) {
                    return !day.button.classList.contains("fully-hidden");
                });

                visibleDays.forEach((day) => {
                    day.button.setAttribute("tabindex", -1);
                    day.button.classList.remove("ds_selected");

                    const dayDate = day.date;
                    dayDate.setHours(0, 0, 0, 0);

                    const today = new Date();
                    today.setHours(0, 0, 0, 0);

                    if (dayDate.getTime() === focusDate.getTime() && !day.disabled) {
                        if (shouldFocus && this.hasUserSelectedDate) {
                            day.button.setAttribute("tabindex", 0);
                            day.button.focus();
                            day.button.classList.add("ds_selected");
                        }
                    }

                    if (this.inputDate && !this.isDisabledDate(this.inputDate) && dayDate.getTime() === this.inputDate.getTime()) {
                        day.button.classList.add("ds_datepicker__current");
                        day.button.setAttribute("aria-description", "selected date");
                    } else {
                        day.button.classList.remove("ds_datepicker__current");
                        day.button.removeAttribute("aria-description");
                    }

                    if (dayDate.getTime() === today.getTime()) {
                        day.button.classList.add("ds_datepicker__today");
                        day.button.setAttribute("aria-current", "date");
                    } else {
                        day.button.classList.remove("ds_datepicker__today");
                        day.button.removeAttribute("aria-current");
                    }
                });

                if (!shouldFocus) {
                    visibleDays[0].button.setAttribute("tabindex", 0);
                    this.currentDate = visibleDays[0].date;
                }
            }

            setDate(date) {
                if (this.isMultipleInput) {
                    this.dateInput.value = this.leadingZeroes(date.getDate());
                    this.monthInput.value = this.leadingZeroes(date.getMonth() + 1);
                    this.yearInput.value = date.getFullYear();
                } else {
                    switch (this.datePickerParent.dataset.dateformat) {
                        case "YMD":
                            this.inputElement.value = `${date.getFullYear()}/${this.leadingZeroes(date.getMonth() + 1)}/${this.leadingZeroes(date.getDate())}`;
                            break;
                        case "MDY":
                            this.inputElement.value = `${this.leadingZeroes(date.getMonth() + 1)}/${this.leadingZeroes(date.getDate())}/${date.getFullYear()}`;
                            break;
                        default:
                            this.inputElement.value = `${this.leadingZeroes(date.getDate())}/${this.leadingZeroes(date.getMonth() + 1)}/${date.getFullYear()}`;
                    }
                }
            }

            setMinAndMaxDatesOnCalendar() {
                if (this.minDate && this.currentDate < this.minDate) {
                    this.currentDate = this.minDate;
                }
                if (this.maxDate && this.currentDate > this.maxDate) {
                    this.currentDate = this.maxDate;
                }
            }

            setOptions() {
                this.transformLegacyDataAttributes();

                if (this.options.minDate) {
                    this.minDate = this.options.minDate;
                } else if (this.datePickerParent.dataset.mindate) {
                    this.minDate = this.formattedDateFromString(this.datePickerParent.dataset.mindate, null);
                }

                if (this.options.maxDate) {
                    this.maxDate = this.options.maxDate;
                } else if (this.datePickerParent.dataset.maxdate) {
                    this.maxDate = this.formattedDateFromString(this.datePickerParent.dataset.maxdate, null);
                }

                if (this.options.dateSelectCallback) {
                    this.dateSelectCallback = this.options.dateSelectCallback;
                }

                if (this.options.disabledDates) {
                    this.disabledDates = this.options.disabledDates;
                } else if (this.datePickerParent.dataset.disableddates) {
                    this.disabledDates = this.datePickerParent.dataset.disableddates
                        .replace(/\s+/, " ")
                        .split(" ")
                        .map((dateStr) => this.formattedDateFromString(dateStr, null))
                        .filter((date) => date);
                }
            }

            toggleDialog(event) {
                event.preventDefault();
                if (this.isOpen()) {
                    this.closeDialog();
                } else {
                    this.setMinAndMaxDatesOnCalendar();
                    this.openDialog();
                }
            }

            transformLegacyDataAttributes() {
                if (this.inputElement.dataset.mindate) {
                    this.datePickerParent.dataset.mindate = this.inputElement.dataset.mindate;
                }
                if (this.inputElement.dataset.maxdate) {
                    this.datePickerParent.dataset.maxdate = this.inputElement.dataset.maxdate;
                }
                if (this.inputElement.dataset.dateformat) {
                    this.datePickerParent.dataset.dateformat = this.inputElement.dataset.dateformat;
                }
            }

            updateCalendar() {
                this.dialogTitleNode.innerHTML = `${this.monthLabels[this.currentDate.getMonth()]} ${this.currentDate.getFullYear()}`;
                this.dialogElement.setAttribute("aria-label", this.dialogTitleNode.innerHTML);

                const currentMonth = this.currentDate;
                const firstOfMonth = new Date(currentMonth.getFullYear(), currentMonth.getMonth(), 1);
                const startDay = firstOfMonth.getDay();
                firstOfMonth.setDate(firstOfMonth.getDate() - startDay);

                const iteratorDate = new Date(firstOfMonth);

                for (const calendarDay of this.calendarDays) {
                    let isDisabled;
                    const isOutsideMonth = iteratorDate.getMonth() !== currentMonth.getMonth();

                    if (iteratorDate < this.minDate) {
                        isDisabled = true;
                    }
                    if (iteratorDate > this.maxDate) {
                        isDisabled = true;
                    }
                    if (this.isDisabledDate(iteratorDate)) {
                        isDisabled = true;
                    }

                    calendarDay.update(iteratorDate, isOutsideMonth, isDisabled);
                    iteratorDate.setDate(iteratorDate.getDate() + 1);
                }

                const rows = this.datePickerParent.querySelectorAll("tbody tr");
                rows.forEach(function (row) {
                    const buttons = [].slice.call(row.querySelectorAll("button"));
                    const allHidden = buttons.every(function (btn) {
                        return btn.classList.contains("fully-hidden");
                    });
                    row.style.display = allHidden ? "none" : "";
                });
            }
        }
    };

    document.addEventListener("DOMContentLoaded", function () {
        [].slice.call(document.querySelectorAll('[data-module="ds-datepicker"]')).forEach(function (element) {
            new window.DS.components.DatePicker(element, {
                imagePath: getWebfilePath("/assets/images/icons/")
            }).init();
        });

        trackingRef.init();
    });
})();