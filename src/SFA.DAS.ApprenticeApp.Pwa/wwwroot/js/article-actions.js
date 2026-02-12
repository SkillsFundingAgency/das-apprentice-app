(function () {
  var actionsContainer = document.querySelector(".app-article-actions");
  if (!actionsContainer) return;

  function getArticleData(button) {
    var section = button.closest(".govuk-accordion__section-content");
    return {
      entryId: section.querySelector(".entryid").value,
      title: section.querySelector(".article-title").value,
    };
  }

  function slugify(title) {
    return title.replace(/\s+/g, "-").toLowerCase();
  }

  function updateArticle(params) {
    var query = new URLSearchParams(params).toString();
    return fetch("/Support/AddOrUpdateApprenticeArticle?" + query);
  }

  // Save
  document.addEventListener("click", function (event) {
    var button = event.target.closest(".app-article-actions__button.save");
    if (!button) return;

    event.preventDefault();
    var data = getArticleData(button);

    updateArticle({
      entryId: data.entryId,
      entryTitle: slugify(data.title),
      isSaved: "true",
    }).then(function () {
      button.classList.remove("save");
      button.classList.add("unsave");
      button.innerHTML =
        '<span class="icon"><svg width="24" height="24"><use href="/assets/icons/sprite.svg#pin-filled"></use></svg></span>' +
        '<span class="text">Remove<span class="govuk-visually-hidden"> "' +
        data.title +
        '" to your saved articles</span></span>';
    });
  });

  // Unsave
  document.addEventListener("click", function (event) {
    var button = event.target.closest(".app-article-actions__button.unsave");
    if (!button) return;

    event.preventDefault();
    var data = getArticleData(button);

    updateArticle({
      entryId: data.entryId,
      entryTitle: slugify(data.title),
      isSaved: "false",
    }).then(function () {
      button.classList.remove("unsave");
      button.classList.add("save");
      button.innerHTML =
        '<span class="icon"><svg width="24" height="24"><use href="/assets/icons/sprite.svg#pin"></use></svg></span>' +
        '<span class="text">Save<span class="govuk-visually-hidden"> "' +
        data.title +
        '" to your saved articles</span></span>';
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
    document.querySelectorAll(".share-btn").forEach(function (btn) {
      btn.hidden = true;
    });
  }
})();
