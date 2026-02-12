(function () {
  var form = document.querySelector('[data-module="task-validation"]');
  if (!form) return;

  var errorSummary = document.getElementById("errorSummary");

  // Detect whether this is a GDS date input (3 fields) or a single date input
  var dayInput = document.getElementById("duedate-day");
  var monthInput = document.getElementById("duedate-month");
  var yearInput = document.getElementById("duedate-year");
  var hiddenDate = document.getElementById("duedate-hidden");
  var isGdsDate = dayInput && monthInput && yearInput;

  var fields = [
    {
      id: "Task_Title",
      errorId: "Task_Title-error",
      rules: [
        { test: function (v) { return !v.trim(); }, message: "Enter a task name" },
        { test: function (v) { return v.trim().length > 150; }, message: "Task name must be 150 characters or less" },
      ],
    },
    {
      id: "time",
      errorId: "time-error",
      rules: [
        { test: function (v) { return !v; }, message: "Enter a time" },
      ],
    },
  ];

  form.addEventListener("submit", function (event) {
    var errors = [];
    clearErrors();

    // Validate standard fields
    fields.forEach(function (field) {
      var input = document.getElementById(field.id);
      if (!input) return;

      for (var i = 0; i < field.rules.length; i++) {
        if (field.rules[i].test(input.value)) {
          errors.push({
            field: input,
            message: field.rules[i].message,
            errorId: field.errorId,
          });
          break;
        }
      }
    });

    // Validate date
    if (isGdsDate) {
      var dateError = validateGdsDate();
      if (dateError) {
        errors.push(dateError);
      } else {
        assembleDateValue();
      }
    } else {
      var singleDate = document.getElementById("date");
      if (singleDate && !singleDate.value) {
        errors.push({
          field: singleDate,
          message: "Enter a date",
          errorId: "date-error",
        });
      }
    }

    if (errors.length > 0) {
      event.preventDefault();
      showErrors(errors);
    } else if (isGdsDate) {
      assembleDateValue();
    }
  });

  function validateGdsDate() {
    var day = dayInput.value.trim();
    var month = monthInput.value.trim();
    var year = yearInput.value.trim();
    var errorInputs = [];

    if (!day && !month && !year) {
      errorInputs = [dayInput, monthInput, yearInput];
      return {
        field: dayInput,
        additionalFields: errorInputs,
        message: "Enter a date",
        errorId: "date-error",
        isDateGroup: true,
      };
    }

    var missing = [];
    if (!day) missing.push("day");
    if (!month) missing.push("month");
    if (!year) missing.push("year");

    if (missing.length > 0) {
      if (!day) errorInputs.push(dayInput);
      if (!month) errorInputs.push(monthInput);
      if (!year) errorInputs.push(yearInput);
      return {
        field: errorInputs[0],
        additionalFields: errorInputs,
        message: "Date must include a " + missing.join(", "),
        errorId: "date-error",
        isDateGroup: true,
      };
    }

    var d = parseInt(day, 10);
    var m = parseInt(month, 10);
    var y = parseInt(year, 10);

    if (isNaN(d) || isNaN(m) || isNaN(y)) {
      errorInputs = [dayInput, monthInput, yearInput];
      return {
        field: dayInput,
        additionalFields: errorInputs,
        message: "Date must be a real date",
        errorId: "date-error",
        isDateGroup: true,
      };
    }

    var testDate = new Date(y, m - 1, d);
    if (testDate.getFullYear() !== y || testDate.getMonth() !== m - 1 || testDate.getDate() !== d) {
      errorInputs = [dayInput, monthInput, yearInput];
      return {
        field: dayInput,
        additionalFields: errorInputs,
        message: "Date must be a real date",
        errorId: "date-error",
        isDateGroup: true,
      };
    }

    return null;
  }

  function assembleDateValue() {
    var d = dayInput.value.trim().padStart(2, "0");
    var m = monthInput.value.trim().padStart(2, "0");
    var y = yearInput.value.trim();
    if (hiddenDate) {
      hiddenDate.value = y + "-" + m + "-" + d;
    }
  }

  function showErrors(errors) {
    var summaryList = errorSummary.querySelector(".govuk-error-summary__list");
    summaryList.innerHTML = "";

    errors.forEach(function (error) {
      var li = document.createElement("li");
      var a = document.createElement("a");
      a.href = "#" + error.field.id;
      a.textContent = error.message;
      li.appendChild(a);
      summaryList.appendChild(li);

      var errorDiv = document.getElementById(error.errorId);
      if (errorDiv) {
        errorDiv.innerHTML =
          '<span class="govuk-visually-hidden">Error:</span> ' +
          error.message;
      }

      if (error.isDateGroup) {
        var dateGroup = document.getElementById("date-group");
        if (dateGroup) {
          dateGroup.classList.add("govuk-form-group--error");
        }
        if (error.additionalFields) {
          error.additionalFields.forEach(function (input) {
            input.classList.add("govuk-input--error");
          });
        }
      } else {
        var formGroup = error.field.closest(".govuk-form-group");
        if (formGroup) {
          formGroup.classList.add("govuk-form-group--error");
        }
        error.field.classList.add("govuk-input--error");
      }
    });

    errorSummary.classList.remove("govuk-visually-hidden");
    errorSummary.removeAttribute("aria-hidden");
    errorSummary.focus();
    errorSummary.scrollIntoView({ behavior: "smooth" });
  }

  function clearErrors() {
    errorSummary.classList.add("govuk-visually-hidden");
    errorSummary.setAttribute("aria-hidden", "true");
    errorSummary.querySelector(".govuk-error-summary__list").innerHTML = "";

    form.querySelectorAll(".govuk-form-group--error").forEach(function (el) {
      el.classList.remove("govuk-form-group--error");
    });

    form.querySelectorAll(".govuk-input--error").forEach(function (el) {
      el.classList.remove("govuk-input--error");
    });

    var errorIds = ["Task_Title-error", "date-error", "time-error"];
    errorIds.forEach(function (id) {
      var el = document.getElementById(id);
      if (el) el.innerHTML = "";
    });
  }
})();
