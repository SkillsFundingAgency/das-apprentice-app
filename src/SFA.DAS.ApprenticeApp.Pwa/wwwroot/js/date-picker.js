(() => {
  "use strict";

  const SVG_CALENDAR_TODAY =
    '<svg width="24" height="24" class="app-icon-item__icon"><use href="/assets/icons/sprite.svg#calendar"></use></svg>';
  const SVG_CHEVRON_LEFT =
    '<svg focusable="false" class="ds_icon" aria-hidden="true" role="img" xmlns="http://www.w3.org/2000/svg" height="24px" viewBox="0 0 24 24" width="24px" fill="#000000"><path d="M0 0h24v24H0z" fill="none"/><path d="M15.41 7.41L14 6l-6 6 6 6 1.41-1.41L10.83 12z"/></svg>';
  const SVG_CHEVRON_RIGHT =
    '<svg focusable="false" class="ds_icon" aria-hidden="true" role="img" xmlns="http://www.w3.org/2000/svg" height="24px" viewBox="0 0 24 24" width="24px" fill="#000000"><path d="M0 0h24v24H0z" fill="none"/><path d="M10 6L8.59 7.41 13.17 12l-4.58 4.59L10 18l6-6z"/></svg>';
  const SVG_DOUBLE_CHEVRON_LEFT =
    '<svg focusable="false" class="ds_icon" aria-hidden="true" role="img" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24"><path d="M19 16.6 17.6 18l-6-6 6-6L19 7.4 14.4 12l4.6 4.6Zm-6.6 0L11 18l-6-6 6-6 1.4 1.4L7.8 12l4.6 4.6Z"/></svg>';
  const SVG_DOUBLE_CHEVRON_RIGHT =
    '<svg focusable="false" class="ds_icon" aria-hidden="true" role="img" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24"><path d="M9.6 12 5 7.4 6.4 6l6 6-6 6L5 16.6 9.6 12Zm6.6 0-4.6-4.6L13 6l6 6-6 6-1.4-1.4 4.6-4.6Z"/></svg>';

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
      this.button.setAttribute(
        "aria-label",
        this.picker.formattedDateHuman(this.date),
      );

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

  window.DS = window.DS || {};
  window.DS.components = window.DS.components || {
    DatePicker: class {
      constructor(element, options = {}) {
        if (!element) return;

        this.datePickerParent = element;
        this.options = options;
        this.inputElement = this.datePickerParent.querySelector("input");
        this.isMultipleInput = element.classList.contains(
          "ds_datepicker--multiple",
        );
        this.dateInput = element.querySelector(".js-datepicker-date");
        this.monthInput = element.querySelector(".js-datepicker-month");
        this.yearInput = element.querySelector(".js-datepicker-year");
        this.dayLabels = [
          "Sunday",
          "Monday",
          "Tuesday",
          "Wednesday",
          "Thursday",
          "Friday",
          "Saturday",
        ];
        this.monthLabels = [
          "January",
          "February",
          "March",
          "April",
          "May",
          "June",
          "July",
          "August",
          "September",
          "October",
          "November",
          "December",
        ];
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
          down: 40,
        };
      }

      init() {
        if (
          !this.inputElement ||
          this.datePickerParent.classList.contains("js-initialised")
        ) {
          return;
        }

        this.setOptions();

        this.icons = {
          calendar_today: SVG_CALENDAR_TODAY,
          chevron_left: SVG_CHEVRON_LEFT,
          chevron_right: SVG_CHEVRON_RIGHT,
          double_chevron_left: SVG_DOUBLE_CHEVRON_LEFT,
          double_chevron_right: SVG_DOUBLE_CHEVRON_RIGHT,
        };

        this.calendarToggleButton = this.datePickerParent.querySelector(
          ".js-calendar-toggle-btn",
        );
        this.calendarContainer = this.datePickerParent.querySelector(
          "#calendar-container",
        );
        this.useInlineMode = !!(
          this.calendarToggleButton && this.calendarContainer
        );

        if (this.useInlineMode) {
          this.calendarButtonElement = document.createElement("button");
          this.calendarButtonElement.type = "button";
          this.calendarButtonElement.style.display = "none";
          this.calendarButtonElement.innerHTML =
            '<span class="visually-hidden">Choose date</span>';
          this.datePickerParent.appendChild(this.calendarButtonElement);
        } else {
          const tempWrapper = document.createElement("div");
          tempWrapper.innerHTML = this.buttonTemplate();
          this.calendarButtonElement = tempWrapper.firstChild;

          if (this.isMultipleInput) {
            const wrapper = document.createElement("div");
            wrapper.className =
              "govuk-date-input__item ds_datepicker__button-wrapper";
            wrapper.appendChild(this.calendarButtonElement);

            const calContainer = this.datePickerParent.querySelector(
              "#calendar-container",
            );
            if (calContainer) {
              this.datePickerParent.insertBefore(wrapper, calContainer);
            } else {
              this.datePickerParent.appendChild(wrapper);
            }
          } else {
            this.inputElement.parentNode.appendChild(
              this.calendarButtonElement,
            );
            this.inputElement.parentNode.classList.add(
              "ds_input__wrapper--has-icon",
            );
          }
        }

        window.DS = window.DS || {};
        window.DS.elementIdModifier = window.DS.elementIdModifier || 0;
        window.DS.elementIdModifier += 1;

        const dialogContainer = document.createElement("div");
        dialogContainer.id = `datepicker-ds${window.DS.elementIdModifier}`;
        dialogContainer.setAttribute(
          "class",
          "ds_datepicker__dialog  datepickerDialog",
        );
        dialogContainer.setAttribute("role", "dialog");
        dialogContainer.setAttribute("aria-modal", "true");
        dialogContainer.innerHTML = this.dialogTemplate(dialogContainer.id);

        this.calendarButtonElement.setAttribute(
          "aria-controls",
          dialogContainer.id,
        );
        this.calendarButtonElement.setAttribute("aria-expanded", false);
        this.dialogElement = dialogContainer;

        if (this.useInlineMode) {
          this.calendarContainer.appendChild(dialogContainer);
          this.dialogElement.classList.add("ds_datepicker__dialog--open");
          this.dialogElement.classList.add("ds_datepicker__dialog--inline");
        } else {
          this.datePickerParent.appendChild(dialogContainer);
        }

        this.dialogTitleNode = this.dialogElement.querySelector(
          ".js-datepicker-month-year",
        );
        this.setMinAndMaxDatesOnCalendar();

        const tableBody = this.datePickerParent.querySelector("tbody");
        let dayIndex = 0;

        for (let rowIndex = 0; rowIndex < 6; rowIndex++) {
          const row = tableBody.insertRow(rowIndex);

          for (let colIndex = 0; colIndex < 7; colIndex++) {
            const cell = document.createElement("td");
            const dayButton = document.createElement("button");
            dayButton.type = "button";
            cell.appendChild(dayButton);
            row.appendChild(cell);

            const calendarDay = new CalendarDay(
              dayButton,
              dayIndex,
              rowIndex,
              colIndex,
              this,
            );
            calendarDay.init();
            this.calendarDays.push(calendarDay);
            dayIndex++;
          }
        }

        this.prevMonthButton = this.dialogElement.querySelector(
          ".js-datepicker-prev-month",
        );
        this.prevYearButton = this.dialogElement.querySelector(
          ".js-datepicker-prev-year",
        );
        this.nextMonthButton = this.dialogElement.querySelector(
          ".js-datepicker-next-month",
        );
        this.nextYearButton = this.dialogElement.querySelector(
          ".js-datepicker-next-year",
        );

        this.prevMonthButton.addEventListener("click", (event) =>
          this.focusPreviousMonth(event, false),
        );
        this.prevYearButton.addEventListener("click", (event) =>
          this.focusPreviousYear(event, false),
        );
        this.nextMonthButton.addEventListener("click", (event) =>
          this.focusNextMonth(event, false),
        );
        this.nextYearButton.addEventListener("click", (event) =>
          this.focusNextYear(event, false),
        );

        [
          this.inputElement,
          this.dateInput,
          this.monthInput,
          this.yearInput,
        ].forEach((input) => {
          if (input) {
            input.addEventListener("blur", () => {
              this.calendarButtonElement.querySelector("span").textContent =
                "Choose date";
            });
          }
        });

        this.cancelButton = this.dialogElement.querySelector(
          ".js-datepicker-cancel",
        );
        this.okButton = this.dialogElement.querySelector(".js-datepicker-ok");

        if (this.useInlineMode) {
          this.cancelButton.addEventListener("click", (event) => {
            event.preventDefault();
            this.calendarContainer.classList.remove(
              "ds_datepicker__calendar-container--open",
            );
            this.calendarToggleButton.setAttribute("aria-expanded", "false");
          });

          this.okButton.addEventListener("click", () => {
            this.selectDate(this.currentDate);
            this.calendarContainer.classList.remove(
              "ds_datepicker__calendar-container--open",
            );
            this.calendarToggleButton.setAttribute("aria-expanded", "false");
          });
        } else {
          this.cancelButton.addEventListener("click", (event) => {
            event.preventDefault();
            this.closeDialog(event);
          });

          this.okButton.addEventListener("click", () =>
            this.selectDate(this.currentDate),
          );
        }

        const allButtons = this.dialogElement.querySelectorAll(
          'button:not([disabled="true"])',
        );
        this.firstButtonInDialog = allButtons[0];
        this.lastButtonInDialog = allButtons[allButtons.length - 1];
        this.firstButtonInDialog.addEventListener("keydown", (event) =>
          this.firstButtonKeyup(event),
        );
        this.lastButtonInDialog.addEventListener("keydown", (event) =>
          this.lastButtonKeyup(event),
        );

        if (this.useInlineMode) {
          this.calendarToggleButton.addEventListener("click", (evt) => {
            evt.preventDefault();
            const isVisible = this.calendarContainer.classList.contains(
              "ds_datepicker__calendar-container--open",
            );

            if (isVisible) {
              this.calendarContainer.classList.remove(
                "ds_datepicker__calendar-container--open",
              );
              this.calendarToggleButton.setAttribute("aria-expanded", "false");
            } else {
              this.setMinAndMaxDatesOnCalendar();
              this.hasUserSelectedDate = false;

              if (this.isMultipleInput) {
                const day = this.dateInput.value.trim();
                const month = this.monthInput.value.trim();
                const year = this.yearInput.value.trim();

                if (day && month && year) {
                  const parsed = new Date(
                    parseInt(year, 10),
                    parseInt(month, 10) - 1,
                    parseInt(day, 10),
                  );
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
              this.calendarContainer.classList.add(
                "ds_datepicker__calendar-container--open",
              );
              this.calendarToggleButton.setAttribute("aria-expanded", "true");
            }
          });
        } else {
          this.calendarButtonElement.addEventListener("click", (event) =>
            this.toggleDialog(event),
          );
        }

        document.body.addEventListener("mouseup", (event) =>
          this.backgroundClick(event),
        );

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
        </button>`;
      }

      dialogTemplate(dialogId) {
        return `<div class="ds_datepicker__dialog__header">
        <div class="ds_datepicker__dialog__navbuttons">
            <button type="button" class="ds_button  ds_button--icon-only  js-datepicker-prev-year" aria-label="previous year">
                <span class="visually-hidden">Previous year</span>
                ${this.icons.double_chevron_left}
            </button>

            <button type="button" class="ds_button  ds_button--icon-only  js-datepicker-prev-month" aria-label="previous month">
                <span class="visually-hidden">Previous month</span>
                ${this.icons.chevron_left}
            </button>
        </div>

        <h2 class="ds_datepicker__dialog__title  js-datepicker-month-year" aria-live="polite">June 2020</h2>

        <div class="ds_datepicker__dialog__navbuttons">
            <button type="button" class="ds_button  ds_button--icon-only  js-datepicker-next-month" aria-label="next month">
                <span class="visually-hidden">Next month</span>
                ${this.icons.chevron_right}
            </button>

            <button type="button" class="ds_button  ds_button--icon-only  js-datepicker-next-year" aria-label="next year">
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
      <button type="button" class="ds_button  ds_button--small  js-datepicker-ok" value="ok">Select</button>
      <button type="button" class="ds_button  ds_button--small  ds_button--cancel  js-datepicker-cancel" value="cancel">Close</button>
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

        if (
          this.isOpen() &&
          !this.dialogElement.contains(event.target) &&
          !this.inputElement.contains(event.target) &&
          !this.calendarButtonElement.contains(event.target)
        ) {
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

        return parsedDate instanceof Date && !isNaN(parsedDate)
          ? parsedDate
          : fallback;
      }

      formattedDateHuman(date) {
        return `${this.dayLabels[date.getDay()]} ${date.getDate()} ${this.monthLabels[date.getMonth()]} ${date.getFullYear()}`;
      }

      goToDate(date, shouldFocus) {
        const previousDate = this.currentDate;
        this.currentDate = date;

        if (
          previousDate.getMonth() !== this.currentDate.getMonth() ||
          previousDate.getFullYear() !== this.currentDate.getFullYear()
        ) {
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
        return this.dialogElement.classList.contains(
          "ds_datepicker__dialog--open",
        );
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
          offsetLeft =
            this.calendarButtonElement.offsetLeft +
            this.calendarButtonElement.offsetWidth +
            16;
          dateValue = `${this.dateInput.value}/${this.monthInput.value}/${this.yearInput.value}`;
        } else {
          offsetLeft = this.inputElement.offsetWidth + 16;
          dateValue = this.inputElement.value;
        }

        const gridUnits = Math.ceil(offsetLeft / 8);

        this.dialogElement.classList.forEach(
          function (cls) {
            if (cls.match(/ds_!_off-l-/)) {
              this.dialogElement.classList.remove(cls);
            }
          }.bind(this),
        );

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

        this.calendarButtonElement.querySelector("span").textContent =
          "Choose date";
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

          if (
            this.inputDate &&
            !this.isDisabledDate(this.inputDate) &&
            dayDate.getTime() === this.inputDate.getTime()
          ) {
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
          this.minDate = this.formattedDateFromString(
            this.datePickerParent.dataset.mindate,
            null,
          );
        }

        if (this.options.maxDate) {
          this.maxDate = this.options.maxDate;
        } else if (this.datePickerParent.dataset.maxdate) {
          this.maxDate = this.formattedDateFromString(
            this.datePickerParent.dataset.maxdate,
            null,
          );
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
          this.datePickerParent.dataset.mindate =
            this.inputElement.dataset.mindate;
        }
        if (this.inputElement.dataset.maxdate) {
          this.datePickerParent.dataset.maxdate =
            this.inputElement.dataset.maxdate;
        }
        if (this.inputElement.dataset.dateformat) {
          this.datePickerParent.dataset.dateformat =
            this.inputElement.dataset.dateformat;
        }
      }

      updateCalendar() {
        this.dialogTitleNode.innerHTML = `${this.monthLabels[this.currentDate.getMonth()]} ${this.currentDate.getFullYear()}`;
        this.dialogElement.setAttribute(
          "aria-label",
          this.dialogTitleNode.innerHTML,
        );

        const currentMonth = this.currentDate;
        const firstOfMonth = new Date(
          currentMonth.getFullYear(),
          currentMonth.getMonth(),
          1,
        );
        const startDay = firstOfMonth.getDay();
        firstOfMonth.setDate(firstOfMonth.getDate() - startDay);

        const iteratorDate = new Date(firstOfMonth);

        for (const calendarDay of this.calendarDays) {
          let isDisabled;
          const isOutsideMonth =
            iteratorDate.getMonth() !== currentMonth.getMonth();

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
    },
  };

  document.addEventListener("DOMContentLoaded", function () {
    [].slice
      .call(document.querySelectorAll('[data-module="ds-datepicker"]'))
      .forEach(function (element) {
        new window.DS.components.DatePicker(element).init();
      });
  });
})();
