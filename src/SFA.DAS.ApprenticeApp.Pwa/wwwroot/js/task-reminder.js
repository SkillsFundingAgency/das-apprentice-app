(function () {
  const fieldset = document.querySelector("[data-reminder-value]");
  if (!fieldset) return;

  const rawValue = fieldset.getAttribute("data-reminder-value");
  const reminderValue = rawValue === "" ? null : parseInt(rawValue, 10);

  const noneRadio = document.getElementById("ReminderValueNone");
  const zeroRadio = document.getElementById("ReminderValueZero");
  const sixtyRadio = document.getElementById("ReminderValueSixty");
  const oneDayRadio = document.getElementById("ReminderValueOneDay");
  const customRadio = document.getElementById("ReminderChoose");
  const lengthInput = document.getElementById("reminder-length");

  initReminderState();

  function initReminderState() {
    [noneRadio, zeroRadio, sixtyRadio, oneDayRadio, customRadio].forEach(
      function (r) {
        if (r) r.checked = false;
      },
    );

    if (reminderValue === null) {
      return;
    }

    if (reminderValue === 0) {
      if (zeroRadio) zeroRadio.checked = true;
    } else if (reminderValue === 60) {
      if (sixtyRadio) sixtyRadio.checked = true;
    } else if (reminderValue === 1440) {
      if (oneDayRadio) oneDayRadio.checked = true;
    } else if (reminderValue > 0) {
      if (customRadio) customRadio.checked = true;
      populateCustomFields(reminderValue);
    }
  }

  function populateCustomFields(value) {
    let unit, length;

    if (value % 1440 === 0) {
      unit = 1440;
      length = value / 1440;
    } else if (value % 60 === 0) {
      unit = 60;
      length = value / 60;
    } else {
      unit = 1;
      length = value;
    }

    if (lengthInput) {
      lengthInput.value = length;
    }

    const unitInputs = document.querySelectorAll('input[name="ReminderUnit"]');
    unitInputs.forEach(function (r) {
      r.checked = false;
    });

    const unitRadio = document.querySelector(
      'input[name="ReminderUnit"][value="' + unit + '"]',
    );
    if (unitRadio) {
      unitRadio.checked = true;
    }
  }

  const form = document.querySelector('[data-module="task-validation"]');
  if (form) {
    form.addEventListener(
      "submit",
      function () {
        if (!customRadio || !customRadio.checked) return;

        const length = parseInt(lengthInput ? lengthInput.value : "", 10);
        const unitRadio = document.querySelector(
          'input[name="ReminderUnit"]:checked',
        );
        const unit = unitRadio ? parseInt(unitRadio.value, 10) : 1;

        if (!isNaN(length) && length > 0 && !isNaN(unit)) {
          customRadio.value = length * unit;
        }
      },
      true,
    );
  }
})();
