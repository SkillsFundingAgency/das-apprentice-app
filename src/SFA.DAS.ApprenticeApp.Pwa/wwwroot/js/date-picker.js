(() => {
    "use strict";
    class t {
        constructor(t, e, a, n, s) {
            this.index = e,
                this.row = a,
                this.column = n,
                this.button = t,
                this.picker = s,
                this.date = new Date
        }
        init() {
            this.button.addEventListener("keydown", this.keyPress.bind(this)),
                this.button.addEventListener("click", this.click.bind(this))
        }
        update(t, e, a) {
            this.date = new Date(t),
                this.button.innerHTML = t.getDate(),
                this.button.setAttribute("aria-label", this.picker.formattedDateHuman(this.date)),
                a ? this.button.setAttribute("aria-disabled", !0) : this.button.removeAttribute("aria-disabled"),
                e ? this.button.classList.add("fully-hidden") : this.button.classList.remove("fully-hidden")
        }
        click(t) {
            this.picker.hasUserSelectedDate = !0,
                this.picker.goToDate(this.date),
                this.picker.setDate(this.date),
                t.stopPropagation(),
                t.preventDefault()
        }
        keyPress(t) {
            let e = !0;
            switch (t.keyCode) {
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
                    t.shiftKey ? this.picker.focusPreviousYear(t) : this.picker.focusPreviousMonth(t);
                    break;
                case this.picker.keycodes.pagedown:
                    t.shiftKey ? this.picker.focusNextYear(t) : this.picker.focusNextMonth(t);
                    break;
                default:
                    e = !1
            }
            e && (t.preventDefault(),
                t.stopPropagation())
        }
    }
    function e(t) {
        return (t = String(t)).trim().toLowerCase().replace(/['"\u2018\u2019\u201C\u201D`]/g, "").replace(/[\W|_]+/g, "-").replace(/^-+|-+$/g, "")
    }
    const a = {
        init: function (t) {
            t || (t = document);
            for (let e in a.add)
                a.add[e](t)
        },
        gatherElements: function (t, e) {
            let a = [].slice.call(e.querySelectorAll(`.${t}`));
            return e.classList && e.classList.contains(t) && a.push(e),
                a
        },
        getClickType: function (t) {
            switch (t.type) {
                case "click":
                    return t.ctrlKey ? "ctrl click" : t.metaKey ? "command/win click" : t.shiftKey ? "shift click" : "primary click";
                case "auxclick":
                    return "middle click";
                case "contextmenu":
                    return "secondary click"
            }
        },
        getNearestSectionHeader: function (t) {
            if ("function" == typeof t.closest && t.closest("nav,.ds_metadata,.ds_summary-card__header"))
                return !1;
            return function (t, e, a) {
                t.reverse(),
                    Element.prototype.matches || (Element.prototype.matches = Element.prototype.msMatchesSelector || Element.prototype.webkitMatchesSelector);
                for (let n = 0, s = t.length; n < s; n++) {
                    if (t[n].matches(e))
                        return t[n];
                    if (a && t[n].matches(a) && t[n].querySelector(e))
                        return t[n].querySelector(e)
                }
            }(function (t) {
                const e = [];
                if (t.parentElement) {
                    const a = [].slice.call(t.parentElement.children);
                    for (let n = 0, s = a.length; n < s && a[n] !== t; n++)
                        e.push(a[n])
                }
                return e
            }(t), "h1,h2,h3,h4,h5,h6,.ds_details__summary", ".ds_page-header,.ds_layout__header,.ds_accordion-item__header") || !!t.parentNode && a.getNearestSectionHeader(t.parentNode)
        },
        pushToDataLayer: function (t) {
            window.dataLayer = window.dataLayer || [],
                window.dataLayer.push(t)
        },
        add: {
            clicks: function (t = document) {
                a.hasAddedClickTracking || (t.addEventListener("click", t => {
                    a.pushToDataLayer({
                        method: a.getClickType(t)
                    })
                }
                ),
                    t.addEventListener("auxclick", t => {
                        1 !== t.button && 4 !== t.buttons || a.pushToDataLayer({
                            method: a.getClickType(t)
                        })
                    }
                    ),
                    t.addEventListener("contextmenu", t => {
                        a.pushToDataLayer({
                            method: a.getClickType(t)
                        })
                    }
                    ),
                    a.hasAddedClickTracking = !0)
            },
            canonicalUrl: () => {
                const t = document.querySelector('link[rel="canonical"]');
                t && t.href && (a.hasAddedCanonicalUrl || (a.pushToDataLayer({
                    canonicalUrl: t.href
                }),
                    a.hasAddedCanonicalUrl = !0))
            }
            ,
            prefersColorScheme: function () {
                if (!window.matchMedia)
                    return;
                const t = window.matchMedia("(prefers-color-scheme: dark)").matches ? "dark" : "light";
                a.hasAddedPrefersColorScheme || (a.pushToDataLayer({
                    prefersColorScheme: t
                }),
                    a.hasAddedPrefersColorScheme = !0)
            },
            version: function () {
                a.hasAddedVersion || (a.pushToDataLayer({
                    version: "v3.4.1"
                }),
                    a.hasAddedVersion = !0)
            },
            accordions: function (t = document) {
                a.gatherElements("ds_accordion", t).forEach(t => {
                    let e = "";
                    if (t.dataset.name && (e = t.dataset.name),
                        !t.classList.contains("js-initialised"))
                        return;
                    [].slice.call(t.querySelectorAll("a:not(.ds_button)")).forEach(t => {
                        t.getAttribute("data-navigation") || t.setAttribute("data-navigation", "accordion-link")
                    }
                    );
                    const a = t.querySelector(".js-open-all")
                        , n = [].slice.call(t.querySelectorAll(".ds_accordion-item"));
                    function s(a) {
                        a && (!function () {
                            const e = t.querySelectorAll(".ds_accordion-item--open").length;
                            return n.length === e
                        }() ? a.setAttribute("data-accordion", `accordion-${e.length ? e + "-" : e}open-all`) : a.setAttribute("data-accordion", `accordion-${e.length ? e + "-" : e}close-all`))
                    }
                    function i(t, a) {
                        const n = t.querySelector(".ds_accordion-item__button")
                            , s = t.querySelector(".ds_accordion-item__control");
                        n.setAttribute("data-accordion", `accordion-${e.length ? e + "-" : e}${s.checked ? "close" : "open"}-${a + 1}`)
                    }
                    s(a),
                        n.forEach((t, e) => {
                            i(t, e)
                        }
                        ),
                        a && a.addEventListener("click", () => {
                            n.forEach((t, e) => {
                                i(t, e)
                            }
                            ),
                                s(a)
                        }
                        ),
                        n.forEach((t, n) => {
                            const i = t.querySelector(".ds_accordion-item__button")
                                , o = t.querySelector(".ds_accordion-item__control");
                            i.addEventListener("click", () => {
                                i.setAttribute("data-accordion", `accordion-${e.length ? e + "-" : e}${o.checked ? "close" : "open"}-${n + 1}`),
                                    s(a)
                            }
                            )
                        }
                        )
                }
                )
            },
            asides: function (t = document) {
                a.gatherElements("ds_article-aside", t).forEach(t => {
                    [].slice.call(t.querySelectorAll("a:not(.ds_button)")).forEach((t, e) => {
                        t.getAttribute("data-navigation") || t.setAttribute("data-navigation", `link-related-${e + 1}`)
                    }
                    )
                }
                )
            },
            autocompletes: function (t = document) {
                function e(t, e) {
                    a.pushToDataLayer({
                        event: "autocomplete",
                        searchText: t,
                        clickText: e.dataset.autocompletetext,
                        resultsCount: parseInt(e.dataset.autocompletecount),
                        clickedResults: `result ${e.dataset.autocompleteposition} of ${e.dataset.autocompletecount}`
                    }),
                        delete e.dataset.autocompletetext,
                        delete e.dataset.autocompletecount,
                        delete e.dataset.autocompleteposition
                }
                a.gatherElements("ds_autocomplete", t).forEach(t => {
                    const a = t.querySelector(".js-autocomplete-input")
                        , n = document.querySelector("#" + a.getAttribute("aria-owns") + " .ds_autocomplete__suggestions-list");
                    let s = a.value;
                    a.addEventListener("keyup", t => {
                        "Enter" === t.key && a.dataset.autocompletetext && e(s, a),
                            s = a.value
                    }
                    ),
                        n.addEventListener("mousedown", () => {
                            e(s, a)
                        }
                        )
                }
                )
            },
            backToTop: function (t = document) {
                a.gatherElements("ds_back-to-top__button", t).forEach(t => {
                    t.setAttribute("data-navigation", "backtotop")
                }
                )
            },
            breadcrumbs: function (t = document) {
                a.gatherElements("ds_breadcrumbs", t).forEach(t => {
                    [].slice.call(t.querySelectorAll(".ds_breadcrumbs__link")).forEach((t, e) => {
                        t.getAttribute("data-navigation") || t.setAttribute("data-navigation", `breadcrumb-${e + 1}`)
                    }
                    )
                }
                )
            },
            buttons: function (t = document) {
                [].slice.call(t.querySelectorAll('.ds_button, input[type="button"], input[type="submit"], button')).forEach(t => {
                    t.getAttribute("data-button") || t.setAttribute("data-button", `button-${e(t.textContent)}`)
                }
                )
            },
            cards: function (t = document) {
                a.gatherElements("ds_card__link--cover", t).forEach((t, e) => {
                    t.getAttribute("data-navigation") || t.setAttribute("data-navigation", `card-${e + 1}`)
                }
                )
            },
            categoryLists: function (t = document) {
                a.gatherElements("ds_category-list", t).forEach(t => {
                    [].slice.call(t.querySelectorAll(".ds_category-item__link")).forEach((t, e) => {
                        t.getAttribute("data-navigation") || t.setAttribute("data-navigation", `category-item-${e + 1}`)
                    }
                    )
                }
                )
            },
            checkboxes: function (t = document) {
                a.gatherElements("ds_checkbox__input", t).forEach(e => {
                    let a = e.getAttribute("data-form");
                    a = !a && e.id ? `checkbox-${e.id}` : a.replace(/-checked/g, ""),
                        e.checked && (a += "-checked"),
                        e.setAttribute("data-form", a),
                        e.id && !e.getAttribute("data-value") && e.setAttribute("data-value", `${e.id}`);
                    const n = t.querySelector(`[for=${e.id}]`);
                    n && !e.classList.contains("js-has-tracking-event") && (n.addEventListener("click", () => {
                        e.dataset.form = `checkbox-${e.id}-${e.checked ? "unchecked" : "checked"}`
                    }
                    ),
                        e.classList.add("js-has-tracking-event"))
                }
                )
            },
            confirmationMessages: function (t = document) {
                a.gatherElements("ds_confirmation-message", t).forEach(t => {
                    [].slice.call(t.querySelectorAll("a:not(.ds_button)")).forEach(t => {
                        t.setAttribute("data-navigation", "confirmation-link")
                    }
                    )
                }
                )
            },
            contactDetails: function (t = document) {
                a.gatherElements("ds_contact-details", t).forEach(t => {
                    [].slice.call(t.querySelectorAll(".ds_contact-details__social-link")).forEach(t => {
                        t.getAttribute("data-navigation") || t.setAttribute("data-navigation", `contact-details-${e(t.textContent)}`)
                    }
                    ),
                        [].slice.call(t.querySelectorAll('a[href^="mailto"]')).forEach(t => {
                            t.getAttribute("data-navigation") || t.setAttribute("data-navigation", "contact-details-email")
                        }
                        )
                }
                )
            },
            contentNavs: function (t = document) {
                a.gatherElements("ds_contents-nav", t).forEach(t => {
                    [].slice.call(t.querySelectorAll(".ds_contents-nav__link")).forEach((t, e) => {
                        t.getAttribute("data-navigation") || t.setAttribute("data-navigation", `contentsnav-${e + 1}`)
                    }
                    )
                }
                )
            },
            details: function (t = document) {
                a.gatherElements("ds_details", t).forEach(t => {
                    const e = t.querySelector(".ds_details__summary");
                    e.setAttribute("data-accordion", "detail-" + (t.open ? "close" : "open")),
                        e.addEventListener("click", () => {
                            e.setAttribute("data-accordion", "detail-" + (t.open ? "open" : "close"))
                        }
                        ),
                        [].slice.call(t.querySelectorAll("a:not(.ds_button)")).forEach(t => {
                            t.getAttribute("data-navigation") || t.setAttribute("data-navigation", "details-link")
                        }
                        )
                }
                )
            },
            errorMessages: function (t = document) {
                a.gatherElements("ds_question__error-message", t).forEach((t, e) => {
                    if ("function" == typeof t.closest && t.closest(".ds_question")) {
                        const a = t.closest(".ds_question").querySelector(".js-validation-group, .ds_input, .ds_select, .ds_checkbox__input, .ds_radio__input");
                        let n = e + 1;
                        if (a)
                            if (a.classList.contains("js-validation-group")) {
                                const t = function (t, e, a) {
                                    return a.indexOf(t) === e
                                };
                                n = [].slice.call(a.querySelectorAll(".ds_input, .ds_select, .ds_checkbox__input, .ds_radio__input")).map(t => "radio" === t.type ? t.name : t.id).filter(t).join("-")
                            } else
                                n = "radio" === a.type ? a.name : a.id;
                        t.getAttribute("data-form") || t.setAttribute("data-form", `error-${n}`)
                    }
                }
                )
            },
            errorSummaries: function (t = document) {
                a.gatherElements("ds_error-summary", t).forEach(t => {
                    [].slice.call(t.querySelectorAll(".ds_error-summary__list a")).forEach(t => {
                        !t.getAttribute("data-form") && t.href && t.setAttribute("data-form", `error-${t.href.substring(t.href.lastIndexOf("#") + 1)}`)
                    }
                    )
                }
                )
            },
            externalLinks: function (t = document) {
                [].slice.call(t.querySelectorAll("a")).filter(t => {
                    let e = window.location.hostname;
                    return window.location.port && (e += ":" + window.location.port),
                        !new RegExp("/" + e + "/?|^tel:|^mailto:|^/").test(t.href)
                }
                ).forEach(t => {
                    t.setAttribute("data-navigation", "link-external")
                }
                )
            },
            hideThisPage: function (t = document) {
                a.gatherElements("ds_hide-page", t).forEach(t => {
                    [].slice.call(t.querySelectorAll(".ds_hide-page__button")).forEach(t => {
                        t.setAttribute("data-navigation", "hide-this-page"),
                            document.addEventListener("keyup", t => {
                                "Escape" !== t.key && 27 !== t.keyCode || a.pushToDataLayer({
                                    event: "hide-this-page-keyboard"
                                })
                            }
                            )
                    }
                    )
                }
                )
            },
            insetTexts: function (t = document) {
                a.gatherElements("ds_inset-text", t).forEach(t => {
                    [].slice.call(t.querySelectorAll(".ds_inset-text__text a:not(.ds_button)")).forEach(t => {
                        t.getAttribute("data-navigation") || t.setAttribute("data-navigation", "inset-link")
                    }
                    )
                }
                )
            },
            links: function () {
                [].slice.call(document.querySelectorAll("a")).forEach(t => {
                    const e = a.getNearestSectionHeader(t);
                    e && (t.getAttribute("data-section") || t.setAttribute("data-section", e.textContent.trim()))
                }
                )
            },
            metadataItems: function (t = document) {
                a.gatherElements("ds_metadata__item", t).forEach((t, a) => {
                    const n = t.querySelector(".ds_metadata__key");
                    let s;
                    s = n ? n.textContent.trim() : `metadata-${a}`,
                        [].slice.call(t.querySelectorAll(".ds_metadata__value a")).forEach((t, a) => {
                            t.getAttribute("data-navigation") || t.setAttribute("data-navigation", `${e(s)}-${a + 1}`)
                        }
                        )
                }
                )
            },
            notifications: function (t = document) {
                a.gatherElements("ds_notification", t).forEach((t, a) => {
                    const n = t.id || a + 1;
                    [].slice.call(t.querySelectorAll("a:not(.ds_button)")).forEach(t => {
                        t.getAttribute("data-banner") || t.setAttribute("data-banner", `banner-${n}-link`)
                    }
                    ),
                        [].slice.call(t.querySelectorAll(".ds_button:not(.ds_notification__close)")).forEach(t => {
                            t.getAttribute("data-banner") || t.setAttribute("data-banner", `banner-${n}-${e(t.textContent)}`)
                        }
                        );
                    const s = t.querySelector(".ds_notification__close");
                    s && !s.getAttribute("data-banner") && s.setAttribute("data-banner", `banner-${n}-close`)
                }
                )
            },
            pagination: function (t = document) {
                a.gatherElements("ds_pagination", t).forEach(t => {
                    const a = t.querySelector(".ds_pagination__load-more button");
                    a && !a.getAttribute("data-search") && a.setAttribute("data-search", "pagination-more"),
                        [].slice.call(t.querySelectorAll("a.ds_pagination__link")).forEach(t => {
                            t.getAttribute("data-search") || t.setAttribute("data-search", `pagination-${e(t.textContent)}`)
                        }
                        )
                }
                )
            },
            phaseBanners: function (t = document) {
                a.gatherElements("ds_phase-banner", t).forEach(t => {
                    const a = t.querySelector(".ds_tag") ? t.querySelector(".ds_tag").textContent.trim() : "phase";
                    [].slice.call(t.querySelectorAll("a")).forEach(t => {
                        t.getAttribute("data-banner") || t.setAttribute("data-banner", `banner-${e(a)}-link`)
                    });
                });
            },
            radios: function (t = document) {
                a.gatherElements("ds_radio__input", t).forEach(t => {
                    !t.getAttribute("data-form") && t.name && t.id && t.setAttribute("data-form", `radio-${t.name}-${t.id}`),
                        t.id && !t.getAttribute("data-value") && t.setAttribute("data-value", `${t.id}`)
                }
                )
            },
            searchFacets: function (t = document) {
                a.gatherElements("ds_facet__button", t).forEach(t => {
                    t.setAttribute("data-button", `button-filter-${t.dataset.slug}-remove`)
                }
                )
            },
            searchResults: function (t = document) {
                a.gatherElements("ds_search-results", t).forEach(t => {
                    const e = t.querySelector(".ds_search-results__list");
                    if (!e)
                        return;
                    const a = [].slice.call(t.querySelectorAll(".ds_search-result"))
                        , n = [].slice.call(t.querySelectorAll(".ds_search-result--promoted"));
                    let s = 1;
                    e.getAttribute("start") && (s = +e.getAttribute("start")),
                        a.forEach((t, a) => {
                            const i = t.querySelector(".ds_search-result__link")
                                , o = t.querySelector(".ds_search-result__media-link")
                                , r = t.querySelector(".ds_search-result__context a");
                            if (t.classList.contains("ds_search-result--promoted")) {
                                let t = `search-promoted-${a + 1}/${n.length}`;
                                i.setAttribute("data-search", t)
                            } else {
                                let t;
                                e.getAttribute("data-total") && (t = e.getAttribute("data-total"));
                                let c = "search-result-" + (s + a - n.length)
                                    , l = "search-image-" + (s + a - n.length)
                                    , d = "search-parent-link-" + (s + a - n.length);
                                t && (c += `/${t}`,
                                    d += `/${t}`),
                                    i.setAttribute("data-search", c),
                                    o && o.setAttribute("data-search", l),
                                    r && r.setAttribute("data-search", d)
                            }
                        }
                        )
                }
                )
            },
            searchSuggestions: function (t = document) {
                a.gatherElements("ds_search-suggestions", t).forEach(t => {
                    const e = [].slice.call(t.querySelectorAll(".ds_search-suggestions a"));
                    e.forEach((t, a) => {
                        t.setAttribute("data-search", `suggestion-result-${a + 1}/${e.length}`)
                    }
                    )
                }
                )
            },
            searchRelated: function (t = document) {
                a.gatherElements("ds_search-results__related", t).forEach(t => {
                    const e = [].slice.call(t.querySelectorAll(".ds_search-results__related a"));
                    e.forEach((t, a) => {
                        t.setAttribute("data-search", `search-related-${a + 1}/${e.length}`)
                    }
                    )
                }
                )
            },
            selects: function (t = document) {
                a.gatherElements("ds_select", t).forEach(t => {
                    !t.getAttribute("data-form") && t.id && t.setAttribute("data-form", `select-${t.id}`),
                        [].slice.call(t.querySelectorAll("option")).forEach(a => {
                            let n = "null";
                            a.value && (n = e(a.value)),
                                a.setAttribute("data-form", `${t.getAttribute("data-form")}-${n}`)
                        }
                        ),
                        t.classList.contains("js-has-tracking-event") || (t.addEventListener("change", t => {
                            a.pushToDataLayer({
                                event: t.target.querySelector(":checked").dataset.form
                            })
                        }
                        ),
                            t.classList.add("js-has-tracking-event"))
                }
                )
            },
            sequentialNavs: function (t = document) {
                a.gatherElements("ds_sequential-nav", t).forEach(t => {
                    const e = t.querySelector(".ds_sequential-nav__item--prev > .ds_sequential-nav__button ")
                        , a = t.querySelector(".ds_sequential-nav__item--next > .ds_sequential-nav__button ");
                    e && !e.getAttribute("data-navigation") && e.setAttribute("data-navigation", "sequential-previous"),
                        a && !a.getAttribute("data-navigation") && a.setAttribute("data-navigation", "sequential-next")
                }
                )
            },
            sideNavs: function (t = document) {
                a.gatherElements("ds_side-navigation", t).forEach(t => {
                    const e = t.querySelector(".ds_side-navigation__list")
                        , a = t.querySelector(".js-side-navigation-button")
                        , n = t.querySelector(".js-toggle-side-navigation");
                    function s() {
                        a.setAttribute("data-navigation", "navigation-" + (n.checked ? "close" : "open"))
                    }
                    !function t(e, a = "") {
                        [].slice.call(e.children).forEach((e, n) => {
                            [].slice.call(e.children).forEach(e => {
                                e.classList.contains("ds_side-navigation__list") ? t(e, `${a}-${n + 1}`) : e.setAttribute("data-navigation", `sidenav${a}-${n + 1}`)
                            }
                            )
                        }
                        )
                    }(e),
                        a && (s(),
                            a.addEventListener("click", () => {
                                s()
                            }
                            ))
                }
                )
            },
            siteBranding: function (t = document) {
                a.gatherElements("ds_site-branding", t).forEach(t => {
                    const e = t.querySelector(".ds_site-branding__logo");
                    e && !e.getAttribute("data-header") && e.setAttribute("data-header", "header-logo");
                    const a = t.querySelector(".ds_site-branding__title");
                    a && !a.getAttribute("data-header") && a.setAttribute("data-header", "header-title")
                }
                )
            },
            siteFooter: function (t = document) {
                a.gatherElements("ds_site-footer", t).forEach(t => {
                    [].slice.call(t.querySelectorAll(".ds_site-footer__org-link")).forEach(t => {
                        t.getAttribute("data-footer") || t.setAttribute("data-footer", "footer-logo")
                    }
                    ),
                        [].slice.call(t.querySelectorAll(".ds_site-footer__copyright a")).forEach(t => {
                            t.getAttribute("data-footer") || t.setAttribute("data-footer", "footer-copyright")
                        }
                        ),
                        [].slice.call(t.querySelectorAll(".ds_site-items__item a:not(.ds_button)")).forEach((t, e) => {
                            t.getAttribute("data-footer") || t.setAttribute("data-footer", `footer-link-${e + 1}`)
                        }
                        )
                }
                )
            },
            siteNavigation: function (t = document) {
                a.gatherElements("ds_site-navigation", t).forEach(t => {
                    [].slice.call(t.querySelectorAll(".ds_site-navigation__link")).forEach((t, e) => {
                        t.getAttribute("data-device") || ("function" == typeof t.closest && t.closest(".ds_site-navigation--mobile") ? t.setAttribute("data-device", "mobile") : t.setAttribute("data-device", "desktop")),
                            t.getAttribute("data-header") || t.setAttribute("data-header", `header-link-${e + 1}`)
                    }
                    )
                }
                ),
                    a.gatherElements("ds_site-navigation--mobile", t).forEach(t => {
                        const e = t.parentNode.querySelector(".js-toggle-menu");
                        e && e.setAttribute("data-header", "header-menu-toggle")
                    }
                    )
            },
            skipLinks: function (t = document) {
                [].slice.call(t.querySelectorAll(".ds_skip-links__link")).forEach((t, e) => {
                    t.getAttribute("data-navigation") || t.setAttribute("data-navigation", `skip-link-${e + 1}`)
                }
                )
            },
            stepNavigation: function (t = document) {
                a.gatherElements("ds_step-navigation", t).forEach(t => {
                    [].slice.call(t.querySelectorAll(".ds_step-navigation__title-link")).forEach(t => {
                        t.setAttribute("data-navigation", "partof-sidebar")
                    }
                    )
                }
                ),
                    a.gatherElements("ds_step-navigation-top", t).forEach(t => {
                        [].slice.call(t.querySelectorAll("a")).forEach(t => {
                            t.setAttribute("data-navigation", "partof-header")
                        }
                        )
                    }
                    )
            },
            summaryCard: function (t = document) {
                a.gatherElements("ds_summary-card", t).forEach((t, a) => {
                    [].slice.call(t.querySelectorAll(".ds_summary-card__actions-list")).forEach(t => {
                        const n = [].slice.call(t.querySelectorAll("button"))
                            , s = [].slice.call(t.querySelectorAll("a"));
                        n.forEach(t => {
                            t.setAttribute("data-button", `button-${e(t.textContent)}-${a + 1}`)
                        }
                        ),
                            s.forEach(t => {
                                t.setAttribute("data-navigation", `navigation-${e(t.textContent)}-${a + 1}`)
                            }
                            )
                    }
                    )
                }
                )
            },
            summaryList: function (t = document) {
                a.gatherElements("ds_summary-list__actions", t).forEach(t => {
                    [].slice.call(t.querySelectorAll("button, a")).forEach(t => {
                        const a = "BUTTON" === t.tagName ? "button" : "navigation"
                            , n = "-" + e(t.closest(".ds_summary-list__item").querySelector(".ds_summary-list__key").textContent);
                        t.setAttribute(`data-${a}`, `${a}-${e(t.textContent)}${n}`)
                    }
                    )
                }
                )
            },
            tabs: function (t = document) {
                const e = a.gatherElements("ds_tabs", t);
                let n = 1;
                e.forEach(t => {
                    [].slice.call(t.querySelectorAll(".ds_tabs__tab-link")).forEach((t, e) => {
                        t.getAttribute("data-navigation") || t.setAttribute("data-navigation", `tab-link-${n}-${e + 1}`)
                    }
                    ),
                        n++
                }
                )
            },
            taskList: function (t = document) {
                a.gatherElements("ds_task-list__task-link", t).forEach(t => {
                    t.getAttribute("data-navigation") || t.setAttribute("data-navigation", "tasklist")
                }
                ),
                    a.gatherElements("js-task-list-skip-link", t).forEach(t => {
                        t.getAttribute("data-navigation") || t.setAttribute("data-navigation", "tasklist-skip")
                    }
                    )
            },
            textInputs: function (t = document) {
                [].slice.call(t.querySelectorAll("input.ds_input")).forEach(t => {
                    if (!t.getAttribute("data-form") && t.id) {
                        const e = t.type;
                        t.setAttribute("data-form", `${e}input-${t.id}`)
                    }
                }
                )
            },
            textareas: function (t = document) {
                [].slice.call(t.querySelectorAll("textarea.ds_input")).forEach(t => {
                    !t.getAttribute("data-form") && t.id && t.setAttribute("data-form", `textarea-${t.id}`)
                }
                )
            },
            warningTexts: function (t = document) {
                a.gatherElements("ds_warning-text", t).forEach(t => {
                    [].slice.call(t.querySelectorAll(".ds_warning-text a:not(.ds_button)")).forEach(t => {
                        t.setAttribute("data-navigation", "warning-link")
                    }
                    )
                }
                )
            }
        }
    }
        , n = a;
    function s(t = "") {
        const e = document.getElementById("br-webfile-path");
        return e ? e.value + t : t
    }
    window.DS = window.DS || {},
        window.DS.components = window.DS.components || {
            DatePicker: class {
                constructor(t, e = {}) {
                    t && (this.datePickerParent = t,
                        this.options = e,
                        this.inputElement = this.datePickerParent.querySelector("input"),
                        this.isMultipleInput = t.classList.contains("ds_datepicker--multiple"),
                        this.dateInput = t.querySelector(".js-datepicker-date"),
                        this.monthInput = t.querySelector(".js-datepicker-month"),
                        this.yearInput = t.querySelector(".js-datepicker-year"),
                        this.dayLabels = ["Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday"],
                        this.monthLabels = ["January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December"],
                        this.currentDate = new Date,
                        this.currentDate.setHours(0, 0, 0, 0),
                        this.calendarDays = [],
                        this.disabledDates = [],
                        this.hasUserSelectedDate = !1,
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
                        })
                }
                init() {
                    if (!this.inputElement || this.datePickerParent.classList.contains("js-initialised"))
                        return;
                    this.setOptions();
                    this.icons = {
                        calendar_today: '<svg width="32" height="35" viewBox="0 0 32 35" fill="none" xmlns="http://www.w3.org/2000/svg"><path d="M8.7373 0.0126953V3.5127H22.7627V0.0126953H26.2373V3.5127H28C28.9592 3.5127 29.7798 3.85402 30.4629 4.53711C31.146 5.2202 31.4873 6.04084 31.4873 7V31.5C31.4873 32.4592 31.146 33.2798 30.4629 33.9629C29.7798 34.646 28.9592 34.9873 28 34.9873H3.5C2.54084 34.9873 1.7202 34.646 1.03711 33.9629C0.354018 33.2798 0.0126953 32.4592 0.0126953 31.5V7C0.0126953 6.04084 0.354018 5.2202 1.03711 4.53711C1.7202 3.85402 2.54084 3.5127 3.5 3.5127H5.2627V0.0126953H8.7373ZM3.4873 31.5127H28.0127V13.9873H3.4873V31.5127ZM8.75 24.5127C9.24269 24.5127 9.65532 24.6788 9.98828 25.0117C10.3212 25.3447 10.4873 25.7573 10.4873 26.25C10.4873 26.7427 10.3212 27.1553 9.98828 27.4883C9.65532 27.8212 9.24269 27.9873 8.75 27.9873C8.25731 27.9873 7.84468 27.8212 7.51172 27.4883C7.17876 27.1553 7.0127 26.7427 7.0127 26.25C7.0127 25.7573 7.17876 25.3447 7.51172 25.0117C7.84468 24.6788 8.25731 24.5127 8.75 24.5127ZM15.75 24.5127C16.2427 24.5127 16.6553 24.6788 16.9883 25.0117C17.3212 25.3447 17.4873 25.7573 17.4873 26.25C17.4873 26.7427 17.3212 27.1553 16.9883 27.4883C16.6553 27.8212 16.2427 27.9873 15.75 27.9873C15.2573 27.9873 14.8447 27.8212 14.5117 27.4883C14.1788 27.1553 14.0127 26.7427 14.0127 26.25C14.0127 25.7573 14.1788 25.3447 14.5117 25.0117C14.8447 24.6788 15.2573 24.5127 15.75 24.5127ZM22.75 24.5127C23.2427 24.5127 23.6553 24.6788 23.9883 25.0117C24.3212 25.3447 24.4873 25.7573 24.4873 26.25C24.4873 26.7427 24.3212 27.1553 23.9883 27.4883C23.6553 27.8212 23.2427 27.9873 22.75 27.9873C22.2573 27.9873 21.8447 27.8212 21.5117 27.4883C21.1788 27.1553 21.0127 26.7427 21.0127 26.25C21.0127 25.7573 21.1788 25.3447 21.5117 25.0117C21.8447 24.6788 22.2573 24.5127 22.75 24.5127ZM8.75 17.5127C9.24269 17.5127 9.65532 17.6788 9.98828 18.0117C10.3212 18.3447 10.4873 18.7573 10.4873 19.25C10.4873 19.7427 10.3212 20.1553 9.98828 20.4883C9.65532 20.8212 9.24269 20.9873 8.75 20.9873C8.25731 20.9873 7.84468 20.8212 7.51172 20.4883C7.17876 20.1553 7.0127 19.7427 7.0127 19.25C7.0127 18.7573 7.17876 18.3447 7.51172 18.0117C7.84468 17.6788 8.25731 17.5127 8.75 17.5127ZM15.75 17.5127C16.2427 17.5127 16.6553 17.6788 16.9883 18.0117C17.3212 18.3447 17.4873 18.7573 17.4873 19.25C17.4873 19.7427 17.3212 20.1553 16.9883 20.4883C16.6553 20.8212 16.2427 20.9873 15.75 20.9873C15.2573 20.9873 14.8447 20.8212 14.5117 20.4883C14.1788 20.1553 14.0127 19.7427 14.0127 19.25C14.0127 18.7573 14.1788 18.3447 14.5117 18.0117C14.8447 17.6788 15.2573 17.5127 15.75 17.5127ZM22.75 17.5127C23.2427 17.5127 23.6553 17.6788 23.9883 18.0117C24.3212 18.3447 24.4873 18.7573 24.4873 19.25C24.4873 19.7427 24.3212 20.1553 23.9883 20.4883C23.6553 20.8212 23.2427 20.9873 22.75 20.9873C22.2573 20.9873 21.8447 20.8212 21.5117 20.4883C21.1788 20.1553 21.0127 19.7427 21.0127 19.25C21.0127 18.7573 21.1788 18.3447 21.5117 18.0117C21.8447 17.6788 22.2573 17.5127 22.75 17.5127Z" fill="#1D70B8" stroke="#1D70B8" stroke-width="0.025"/></svg>',
                        chevron_left: '<svg focusable="false" class="ds_icon" aria-hidden="true" role="img" xmlns="http://www.w3.org/2000/svg" height="24px" viewBox="0 0 24 24" width="24px" fill="#000000"><path d="M0 0h24v24H0z" fill="none"/><path d="M15.41 7.41L14 6l-6 6 6 6 1.41-1.41L10.83 12z"/></svg>',
                        chevron_right: '<svg focusable="false" class="ds_icon" aria-hidden="true" role="img" xmlns="http://www.w3.org/2000/svg" height="24px" viewBox="0 0 24 24" width="24px" fill="#000000"><path d="M0 0h24v24H0z" fill="none"/><path d="M10 6L8.59 7.41 13.17 12l-4.58 4.59L10 18l6-6z"/></svg>',
                        double_chevron_left: '<svg focusable="false" class="ds_icon" aria-hidden="true" role="img" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24"><path d="M19 16.6 17.6 18l-6-6 6-6L19 7.4 14.4 12l4.6 4.6Zm-6.6 0L11 18l-6-6 6-6 1.4 1.4L7.8 12l4.6 4.6Z"/></svg>',
                        double_chevron_right: '<svg focusable="false" class="ds_icon" aria-hidden="true" role="img" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24"><path d="M9.6 12 5 7.4 6.4 6l6 6-6 6L5 16.6 9.6 12Zm6.6 0-4.6-4.6L13 6l6 6-6 6-1.4-1.4 4.6-4.6Z"/></svg>'
                    };

                    // Look for the calendar icon button already in the HTML (placed next to date inputs)
                    this.calendarToggleButton = this.datePickerParent.querySelector(".js-calendar-toggle-btn");

                    // Look for the calendar container div in the HTML
                    this.calendarContainer = this.datePickerParent.querySelector("#calendar-container");

                    // Determine if we use the inline icon-toggle mode (icon button + container exist in HTML)
                    this.useInlineMode = !!(this.calendarToggleButton && this.calendarContainer);

                    // Create the internal calendar button reference used by the picker logic
                    if (this.useInlineMode) {
                        // Use the existing icon button as the toggle; create a hidden dummy for internal references
                        this.calendarButtonElement = document.createElement("button");
                        this.calendarButtonElement.type = "button";
                        this.calendarButtonElement.style.display = "none";
                        this.calendarButtonElement.innerHTML = '<span class="visually-hidden">Choose date</span>';
                        this.datePickerParent.appendChild(this.calendarButtonElement);
                    } else {
                        const e = document.createElement("div");
                        e.innerHTML = this.buttonTemplate();
                        this.calendarButtonElement = e.firstChild;
                        this.calendarButtonElement.setAttribute("data-button", `datepicker-${this.inputElement.id}-toggle`);
                        if (this.isMultipleInput) {
                            // Wrap button in a date-input item centered with the input fields
                            const wrapper = document.createElement("div");
                            wrapper.className = "govuk-date-input__item";
                            wrapper.style.alignSelf = "flex-end";
                            wrapper.style.marginLeft = "1px";
                            // Remove any default styling from the button itself
                            this.calendarButtonElement.style.background = "none";
                            this.calendarButtonElement.style.border = "none";
                            this.calendarButtonElement.style.padding = "0";
                            this.calendarButtonElement.style.cursor = "pointer";
                            this.calendarButtonElement.style.lineHeight = "0";
                            // Nudge icon down to vertically center with input boxes (input height 40px, icon 35px)
                            this.calendarButtonElement.style.paddingTop = "3px";
                            wrapper.appendChild(this.calendarButtonElement);
                            // Insert the wrapper before #calendar-container so it sits in the input row
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

                    // Build the dialog element
                    const a = document.createElement("div");
                    a.id = "datepicker-" + (window.DS = window.DS || {},
                        window.DS.elementIdModifier = window.DS.elementIdModifier || 0,
                        window.DS.elementIdModifier += 1,
                        `ds${window.DS.elementIdModifier}`);
                    a.setAttribute("class", "ds_datepicker__dialog  datepickerDialog");
                    a.setAttribute("role", "dialog");
                    a.setAttribute("aria-modal", "true");
                    a.innerHTML = this.dialogTemplate(a.id);
                    this.calendarButtonElement.setAttribute("aria-controls", a.id);
                    this.calendarButtonElement.setAttribute("aria-expanded", !1);
                    this.dialogElement = a;

                    // Place dialog: inside calendar-container (inline) or appended to parent (popup)
                    if (this.useInlineMode) {
                        this.calendarContainer.appendChild(a);
                        this.dialogElement.classList.add("ds_datepicker__dialog--open");
                        this.dialogElement.style.position = "static";
                        this.dialogElement.style.boxShadow = "none";
                        this.dialogElement.style.border = "none";
                    } else {
                        this.datePickerParent.appendChild(a);
                    }

                    this.dialogTitleNode = this.dialogElement.querySelector(".js-datepicker-month-year");
                    this.setMinAndMaxDatesOnCalendar();

                    // Build the 6x7 calendar day grid
                    const n = this.datePickerParent.querySelector("tbody");
                    let s = 0;
                    for (let e = 0; e < 6; e++) {
                        const a = n.insertRow(e);
                        for (let n = 0; n < 7; n++) {
                            const i = document.createElement("td"),
                                o = document.createElement("button");
                            o.type = "button";
                            o.dataset.form = "date-select";
                            i.appendChild(o);
                            a.appendChild(i);
                            const r = new t(o, s, e, n, this);
                            r.init();
                            this.calendarDays.push(r);
                            s++;
                        }
                    }

                    // Navigation buttons
                    this.prevMonthButton = this.dialogElement.querySelector(".js-datepicker-prev-month");
                    this.prevYearButton = this.dialogElement.querySelector(".js-datepicker-prev-year");
                    this.nextMonthButton = this.dialogElement.querySelector(".js-datepicker-next-month");
                    this.nextYearButton = this.dialogElement.querySelector(".js-datepicker-next-year");
                    this.prevMonthButton.addEventListener("click", t => this.focusPreviousMonth(t, !1));
                    this.prevYearButton.addEventListener("click", t => this.focusPreviousYear(t, !1));
                    this.nextMonthButton.addEventListener("click", t => this.focusNextMonth(t, !1));
                    this.nextYearButton.addEventListener("click", t => this.focusNextYear(t, !1));

                    // Blur handlers for inputs
                    [this.inputElement, this.dateInput, this.monthInput, this.yearInput].forEach(t => {
                        t && t.addEventListener("blur", () => {
                            this.calendarButtonElement.querySelector("span").textContent = "Choose date";
                        });
                    });

                    // Cancel and OK buttons
                    this.cancelButton = this.dialogElement.querySelector(".js-datepicker-cancel");
                    this.okButton = this.dialogElement.querySelector(".js-datepicker-ok");

                    if (this.useInlineMode) {
                        this.cancelButton.addEventListener("click", t => {
                            t.preventDefault();
                            this.calendarContainer.style.display = "none";
                            this.calendarToggleButton.setAttribute("aria-expanded", "false");
                        });
                        this.okButton.addEventListener("click", () => {
                            this.selectDate(this.currentDate);
                            this.calendarContainer.style.display = "none";
                            this.calendarToggleButton.setAttribute("aria-expanded", "false");
                        });
                    } else {
                        this.cancelButton.addEventListener("click", t => {
                            t.preventDefault(),
                                this.closeDialog(t)
                        }
                        ),
                            this.okButton.addEventListener("click", () => this.selectDate(this.currentDate));
                    }

                    // Focus trap within the dialog
                    const i = this.dialogElement.querySelectorAll('button:not([disabled="true"])');
                    this.firstButtonInDialog = i[0];
                    this.lastButtonInDialog = i[i.length - 1];
                    this.firstButtonInDialog.addEventListener("keydown", t => this.firstButtonKeyup(t));
                    this.lastButtonInDialog.addEventListener("keydown", t => this.lastButtonKeyup(t));

                    // Wire toggle: calendar icon button click (inline) or calendar button click (popup)
                    if (this.useInlineMode) {
                        this.calendarToggleButton.addEventListener("click", (evt) => {
                            evt.preventDefault();
                            const isVisible = this.calendarContainer.style.display !== "none";
                            if (isVisible) {
                                this.calendarContainer.style.display = "none";
                                this.calendarToggleButton.setAttribute("aria-expanded", "false");
                            } else {
                                this.setMinAndMaxDatesOnCalendar();
                                this.hasUserSelectedDate = !1;
                                if (this.isMultipleInput) {
                                    const d = this.dateInput.value.trim();
                                    const m = this.monthInput.value.trim();
                                    const y = this.yearInput.value.trim();
                                    if (d && m && y) {
                                        const parsed = new Date(parseInt(y, 10), parseInt(m, 10) - 1, parseInt(d, 10));
                                        if (!isNaN(parsed.getTime())) {
                                            this.inputDate = parsed;
                                            this.currentDate = new Date(parsed);
                                            this.hasUserSelectedDate = !0;
                                        }
                                    } else {
                                        this.inputDate = null;
                                    }
                                }
                                this.updateCalendar();
                                this.setCurrentDate(!1);
                                this.calendarContainer.style.display = "";
                                this.calendarToggleButton.setAttribute("aria-expanded", "true");
                            }
                        });
                    } else {
                        this.calendarButtonElement.addEventListener("click", t => this.toggleDialog(t));
                    }

                    // Close on background click (only for popup mode)
                    document.body.addEventListener("mouseup", t => this.backgroundClick(t));

                    this.updateCalendar();
                    this.datePickerParent.classList.add("js-initialised")
                }
                addMonths(t, e) {
                    const a = t.getDate();
                    return t.setMonth(t.getMonth() + +e),
                        t.getDate() !== a && t.setDate(0),
                        t
                }
                buttonTemplate() {
                    return `<button type="button" class="ds_button  ds_button--icon-only  ds_datepicker__button  ds_no-margin  js-calendar-button" aria-expanded="false">\n            <span class="visually-hidden">Choose date</span>\n            ${this.icons.calendar_today}\n        </button>\n        `
                }
                dialogTemplate(t) {
                    return `<div class="ds_datepicker__dialog__header">\n        <div class="ds_datepicker__dialog__navbuttons">\n            <button type="button" class="ds_button  ds_button--icon-only  js-datepicker-prev-year" aria-label="previous year" data-button="button-datepicker-prevyear">\n                <span class="visually-hidden">Previous year</span>\n                ${this.icons.double_chevron_left}\n            </button>\n\n            <button type="button" class="ds_button  ds_button--icon-only  js-datepicker-prev-month" aria-label="previous month" data-button="button-datepicker-prevmonth">\n                <span class="visually-hidden">Previous month</span>\n                ${this.icons.chevron_left}\n            </button>\n        </div>\n\n        <h2 class="ds_datepicker__dialog__title  js-datepicker-month-year" aria-live="polite">June 2020</h2>\n\n        <div class="ds_datepicker__dialog__navbuttons">\n            <button type="button" class="ds_button  ds_button--icon-only  js-datepicker-next-month" aria-label="next month" data-button="button-datepicker-nextmonth">\n                <span class="visually-hidden">Next month</span>\n                ${this.icons.chevron_right}\n            </button>\n\n            <button type="button" class="ds_button  ds_button--icon-only  js-datepicker-next-year" aria-label="next year" data-button="button-datepicker-nextyear">\n                <span class="visually-hidden">Next year</span>\n                ${this.icons.double_chevron_right}\n            </button>\n        </div>\n      </div>\n\n      <table class="ds_datepicker__dialog__table  js-datepicker-grid" role="grid">\n      <thead>\n          <tr>\n          <th scope="col">\n            <span aria-hidden="true">Sun</span>\n            <span class="visually-hidden">Sunday</span>\n          </th>\n          <th scope="col">\n            <span aria-hidden="true">Mon</span>\n            <span class="visually-hidden">Monday</span>\n          </th>\n          <th scope="col">\n            <span aria-hidden="true">Tue</span>\n            <span class="visually-hidden">Tuesday</span>\n          </th>\n          <th scope="col">\n            <span aria-hidden="true">Wed</span>\n            <span class="visually-hidden">Wednesday</span>\n          </th>\n          <th scope="col">\n            <span aria-hidden="true">Thu</span>\n            <span class="visually-hidden">Thursday</span>\n          </th>\n          <th scope="col">\n            <span aria-hidden="true">Fri</span>\n            <span class="visually-hidden">Friday</span>\n          </th>\n          <th scope="col">\n            <span aria-hidden="true">Sat</span>\n            <span class="visually-hidden">Saturday</span>\n          </th>\n          </tr>\n      </thead>\n\n      <tbody></tbody>\n      </table>\n\n      <div class="ds_datepicker__dialog__buttongroup">\n      <button type="button" class="ds_button  ds_button--small  js-datepicker-ok" value="ok" data-button="button-datepicker-ok">Select</button>\n      <button type="button" class="ds_button  ds_button--small  ds_button--cancel  js-datepicker-cancel" value="cancel" data-button="button-datepicker-cancel">Close</button>\n      </div>`
                }
                leadingZeroes(t, e = 2) {
                    let a = t.toString();
                    for (; a.length < e;)
                        a = "0" + a.toString();
                    return a
                }
                backgroundClick(t) {
                    if (this.useInlineMode) return;
                    !this.isOpen() || this.dialogElement.contains(t.target) || this.inputElement.contains(t.target) || this.calendarButtonElement.contains(t.target) || (t.preventDefault(),
                        this.closeDialog())
                }
                closeDialog() {
                    this.dialogElement.classList.remove("ds_datepicker__dialog--open"),
                        this.calendarButtonElement.setAttribute("aria-expanded", !1),
                        this.calendarButtonElement.focus()
                }
                firstButtonKeyup(t) {
                    t.keyCode === this.keycodes.tab && t.shiftKey && (this.lastButtonInDialog.focus(),
                        t.preventDefault())
                }
                focusNextDay(t = new Date(this.currentDate)) {
                    t.setDate(t.getDate() + 1),
                        this.goToDate(t)
                }
                focusPreviousDay(t = new Date(this.currentDate)) {
                    t.setDate(t.getDate() - 1),
                        this.goToDate(t)
                }
                focusNextWeek(t = new Date(this.currentDate)) {
                    t.setDate(t.getDate() + 7),
                        this.goToDate(t)
                }
                focusPreviousWeek(t = new Date(this.currentDate)) {
                    t.setDate(t.getDate() - 7),
                        this.goToDate(t)
                }
                focusFirstDayOfWeek() {
                    const t = new Date(this.currentDate);
                    t.setDate(t.getDate() - t.getDay()),
                        this.goToDate(t)
                }
                focusLastDayOfWeek() {
                    const t = new Date(this.currentDate);
                    t.setDate(t.getDate() - t.getDay() + 6),
                        this.goToDate(t)
                }
                focusNextMonth(t, e = !0) {
                    t.preventDefault();
                    const a = new Date(this.currentDate);
                    this.addMonths(a, 1),
                        this.goToDate(a, e)
                }
                focusPreviousMonth(t, e = !0) {
                    t.preventDefault();
                    const a = new Date(this.currentDate);
                    this.addMonths(a, -1),
                        this.goToDate(a, e)
                }
                focusNextYear(t, e = !0) {
                    t.preventDefault();
                    const a = new Date(this.currentDate);
                    a.setFullYear(a.getFullYear() + 1),
                        this.goToDate(a, e)
                }
                focusPreviousYear(t, e = !0) {
                    t.preventDefault();
                    const a = new Date(this.currentDate);
                    a.setFullYear(a.getFullYear() - 1),
                        this.goToDate(a, e)
                }
                formattedDateFromString(t, e = new Date) {
                    let a = null;
                    const n = t.split("/");
                    if (t.match(/\d{1,4}\/\d{1,2}\/\d{1,4}/))
                        switch (this.datePickerParent.dataset.dateformat) {
                            case "YMD":
                                a = new Date(`${n[1]}/${n[2]}/${n[0]}`);
                                break;
                            case "MDY":
                                a = new Date(`${n[0]}/${n[1]}/${n[2]}`);
                                break;
                            default:
                                a = new Date(`${n[1]}/${n[0]}/${n[2]}`)
                        }
                    return a instanceof Date && !isNaN(a) ? a : e
                }
                formattedDateHuman(t) {
                    return `${this.dayLabels[t.getDay()]} ${t.getDate()} ${this.monthLabels[t.getMonth()]} ${t.getFullYear()}`
                }
                goToDate(t, e) {
                    const a = this.currentDate;
                    this.currentDate = t,
                        a.getMonth() === this.currentDate.getMonth() && a.getFullYear() === this.currentDate.getFullYear() || this.updateCalendar(),
                        this.setCurrentDate(e)
                }
                isDisabledDate(t) {
                    let e = !1;
                    this.minDate && this.minDate > t && (e = !0),
                        this.maxDate && this.maxDate < t && (e = !0);
                    for (const a of this.disabledDates)
                        t.toDateString() === a.toDateString() && (e = !0);
                    return e
                }
                isOpen() {
                    return this.dialogElement.classList.contains("ds_datepicker__dialog--open")
                }
                lastButtonKeyup(t) {
                    t.keyCode !== this.keycodes.tab || t.shiftKey || (this.firstButtonInDialog.focus(),
                        t.preventDefault())
                }
                openDialog() {
                    let t, e;
                    this.dialogElement.classList.add("ds_datepicker__dialog--open"),
                        this.calendarButtonElement.setAttribute("aria-expanded", !0),
                        this.isMultipleInput ? (t = this.calendarButtonElement.offsetLeft + this.calendarButtonElement.offsetWidth + 16,
                            e = `${this.dateInput.value}/${this.monthInput.value}/${this.yearInput.value}`) : (t = this.inputElement.offsetWidth + 16,
                                e = this.inputElement.value);
                    const a = Math.ceil(t / 8);
                    this.dialogElement.classList.forEach(t => {
                        t.match(/ds_!_off-l-/) && this.dialogElement.classList.remove(t)
                    }
                    ),
                        this.dialogElement.classList.add(`ds_!_off-l-${a}`);
                    // Only treat as a pre-selected date if the input actually has a complete date
                    if (e.match(/^\d{1,2}\/\d{1,2}\/\d{4}$/)) {
                        this.inputDate = this.formattedDateFromString(e);
                        this.currentDate = this.inputDate;
                        this.hasUserSelectedDate = !0;
                    } else {
                        this.inputDate = null;
                        this.hasUserSelectedDate = !1;
                    }
                    this.updateCalendar(),
                        this.setCurrentDate()
                }
                selectDate(t) {
                    if (this.isDisabledDate(t))
                        return !1;
                    this.calendarButtonElement.querySelector("span").textContent = "Choose date",
                        this.setDate(t),
                        this.hasUserSelectedDate = !0;
                    const e = document.createEvent("Event");
                    e.initEvent("change", !0, !0),
                        this.inputElement.dispatchEvent(e),
                        this.dateSelectCallback && this.dateSelectCallback(t),
                        // Only close the popup dialog in non-inline mode
                        this.useInlineMode || this.closeDialog();
                }
                setCurrentDate(t = !0) {
                    const e = this.currentDate
                        , a = this.calendarDays.filter(t => !1 === t.button.classList.contains("fully-hidden"));
                    a.forEach(a => {
                        a.button.setAttribute("tabindex", -1),
                            a.button.classList.remove("ds_selected");
                        const n = a.date;
                        n.setHours(0, 0, 0, 0);
                        const s = new Date;
                        s.setHours(0, 0, 0, 0),
                            n.getTime() !== e.getTime() || a.disabled || t && this.hasUserSelectedDate && (a.button.setAttribute("tabindex", 0),
                                a.button.focus(),
                                a.button.classList.add("ds_selected")),
                            this.inputDate && !this.isDisabledDate(this.inputDate) && n.getTime() === this.inputDate.getTime() ? (a.button.classList.add("ds_datepicker__current"),
                                a.button.setAttribute("aria-description", "selected date")) : (a.button.classList.remove("ds_datepicker__current"),
                                    a.button.removeAttribute("aria-description")),
                            n.getTime() === s.getTime() ? (a.button.classList.add("ds_datepicker__today"),
                                a.button.setAttribute("aria-current", "date")) : (a.button.classList.remove("ds_datepicker__today"),
                                    a.button.removeAttribute("aria-current"))
                    }
                    ),
                        t || (a[0].button.setAttribute("tabindex", 0),
                            this.currentDate = a[0].date)
                }
                setDate(t) {
                    if (this.isMultipleInput)
                        this.dateInput.value = this.leadingZeroes(t.getDate()),
                            this.monthInput.value = this.leadingZeroes(t.getMonth() + 1),
                            this.yearInput.value = t.getFullYear();
                    else
                        switch (this.inputElement.value = `${this.leadingZeroes(t.getDate())}/${this.leadingZeroes(t.getMonth() + 1)}/${t.getFullYear()}`,
                        this.datePickerParent.dataset.dateformat) {
                            case "YMD":
                                this.inputElement.value = `${t.getFullYear()}/${this.leadingZeroes(t.getMonth() + 1)}/${this.leadingZeroes(t.getDate())}`;
                                break;
                            case "MDY":
                                this.inputElement.value = `${this.leadingZeroes(t.getMonth() + 1)}/${this.leadingZeroes(t.getDate())}/${t.getFullYear()}`;
                                break;
                            default:
                                this.inputElement.value = `${this.leadingZeroes(t.getDate())}/${this.leadingZeroes(t.getMonth() + 1)}/${t.getFullYear()}`
                        }
                }
                setMinAndMaxDatesOnCalendar() {
                    this.minDate && this.currentDate < this.minDate && (this.currentDate = this.minDate),
                        this.maxDate && this.currentDate > this.maxDate && (this.currentDate = this.maxDate)
                }
                setOptions() {
                    this.transformLegacyDataAttributes(),
                        this.options.minDate ? this.minDate = this.options.minDate : this.datePickerParent.dataset.mindate && (this.minDate = this.formattedDateFromString(this.datePickerParent.dataset.mindate, null)),
                        this.options.maxDate ? this.maxDate = this.options.maxDate : this.datePickerParent.dataset.maxdate && (this.maxDate = this.formattedDateFromString(this.datePickerParent.dataset.maxdate, null)),
                        this.options.dateSelectCallback && (this.dateSelectCallback = this.options.dateSelectCallback),
                        this.options.disabledDates ? this.disabledDates = this.options.disabledDates : this.datePickerParent.dataset.disableddates && (this.disabledDates = this.datePickerParent.dataset.disableddates.replace(/\s+/, " ").split(" ").map(t => this.formattedDateFromString(t, null)).filter(t => t))
                }
                toggleDialog(t) {
                    t.preventDefault(),
                        this.isOpen() ? this.closeDialog() : (this.setMinAndMaxDatesOnCalendar(),
                            this.openDialog())
                }
                transformLegacyDataAttributes() {
                    this.inputElement.dataset.mindate && (this.datePickerParent.dataset.mindate = this.inputElement.dataset.mindate),
                        this.inputElement.dataset.maxdate && (this.datePickerParent.dataset.maxdate = this.inputElement.dataset.maxdate),
                        this.inputElement.dataset.dateformat && (this.datePickerParent.dataset.dateformat = this.inputElement.dataset.dateformat)
                }
                updateCalendar() {
                    this.dialogTitleNode.innerHTML = `${this.monthLabels[this.currentDate.getMonth()]} ${this.currentDate.getFullYear()}`,
                        this.dialogElement.setAttribute("aria-label", this.dialogTitleNode.innerHTML);
                    let t = this.currentDate;
                    const e = new Date(t.getFullYear(), t.getMonth(), 1)
                        , a = e.getDay();
                    e.setDate(e.getDate() - a);
                    const n = new Date(e);
                    for (const e of this.calendarDays) {
                        let a, s = n.getMonth() !== t.getMonth();
                        n < this.minDate && (a = !0),
                            n > this.maxDate && (a = !0),
                            this.isDisabledDate(n) && (a = !0),
                            e.update(n, s, a),
                            n.setDate(n.getDate() + 1)
                    }
                    const o = this.datePickerParent.querySelectorAll("tbody tr");
                    o.forEach(t => {
                        const e = [].slice.call(t.querySelectorAll("button"));
                        const a = e.every(t => t.classList.contains("fully-hidden"));
                        t.style.display = a ? "none" : "";
                    })
                }
            }
        },
        document.addEventListener("DOMContentLoaded", () => {
            [].slice.call(document.querySelectorAll('[data-module="ds-datepicker"]')).forEach(t => {
                new window.DS.components.DatePicker(t, {
                    imagePath: s("/assets/images/icons/")
                }).init()
            }),
                n.init()
        })
})();
