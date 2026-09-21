(function () {
  var actionsContainer = document.querySelector(".app-article-actions");
  if (!actionsContainer) return;

  function escapeHtml(value) {
    var element = document.createElement("div");
    element.textContent = value;
    return element.innerHTML;
  }

  function buttonContent(isSaved, title) {
    var icon = isSaved ? "pin-filled" : "pin";
    var action = isSaved ? "Remove" : "Save";
    var hiddenText = isSaved
      ? '" from your saved articles'
      : '" to your saved articles';

    return (
      '<span class="icon"><svg width="24" height="24"><use href="/assets/icons/sprite.svg#' +
      icon +
      '"></use></svg></span>' +
      '<span class="text">' +
      action +
      '<span class="govuk-visually-hidden"> "' +
      escapeHtml(title) +
      hiddenText +
      "</span></span>"
    );
  }

  // Save and remove. The form works on its own without JavaScript; here we post
  // it in the background and update the button in place instead of reloading.
  document.addEventListener("submit", function (event) {
    var form = event.target.closest(".app-article-actions__form");
    if (!form) return;

    event.preventDefault();

    var button = form.querySelector(".app-article-actions__button");
    var isSavedInput = form.querySelector("[name='isSaved']");
    var title = form.querySelector("[name='entryTitle']").value;
    var saving = isSavedInput.value === "true";

    fetch(form.action, {
      method: "POST",
      headers: { "X-Requested-With": "XMLHttpRequest" },
      body: new FormData(form),
    }).then(function () {
      // The button now offers the opposite action to the one just taken.
      isSavedInput.value = saving ? "false" : "true";
      button.classList.toggle("save", !saving);
      button.classList.toggle("unsave", saving);
      button.innerHTML = buttonContent(saving, title);
    });
  });

  // Share
  if (navigator.share) {
    document.addEventListener("click", function (event) {
      var button = event.target.closest(
        ".app-article-actions__button.share-btn",
      );
      if (!button) return;

      event.preventDefault();
      var section = button.closest(".govuk-accordion__section-content");
      var title = section.querySelector(".article-title").value;
      var body = section.querySelector(".govuk-body");

      navigator.share({
        title: title,
        text: body ? body.textContent : "",
      });
    });
  } else {
    // Hide the wrapper rather than the button, so the flex row does not keep a
    // gap where the share button used to be.
    document.querySelectorAll(".share-btn").forEach(function (btn) {
      var wrapper = btn.closest(".show-if-js-enabled");
      (wrapper || btn).hidden = true;
    });
  }
})();
