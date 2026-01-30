function AutoComplete(selectField) {
    this.selectElement = selectField
}

AutoComplete.prototype.init = function () {
    this.autoComplete()
}

AutoComplete.prototype.getSuggestions = function (query, updateResults) {
    let results = [];
    let apiUrl = "/providers/registeredProviders?query=" + query
    let xhr = new XMLHttpRequest();
    xhr.onreadystatechange = function () {
        if (xhr.readyState === 4) {
            let jsonResponse = JSON.parse(xhr.responseText);
            results = jsonResponse.map(function (result) {
                return result
            });
            updateResults(results);
        }
    }
    xhr.open("GET", apiUrl, true);
    xhr.send();
}

AutoComplete.prototype.getStandards = function (query, updateResults) {
    let results = [];
    let apiUrl = "/providers/standards?query=" + query
    let xhr = new XMLHttpRequest();
    xhr.onreadystatechange = function () {
        if (xhr.readyState === 4) {
            let jsonResponse = JSON.parse(xhr.responseText);
            results = jsonResponse.map(function (result) {
                return result
            });
            updateResults(results);
        }
    }
    xhr.open("GET", apiUrl, true);
    xhr.send();
}


AutoComplete.prototype.onConfirm = function (option) {
    // Populate form fields with selected option
    document.getElementById("Name").value = option.name;
    /*document.getElementById("Ukprn").value = option.ukprn;    */
}

AutoComplete.prototype.onStandardsConfirm = function (option) {
    // Populate form fields with selected option    
    document.getElementById("Title").value = option.title;
    /*document.getElementById("StandardUId").value = option.standardUId;*/
}

function standardTemplate(result) {
    return result && [result.title].filter(element => element).join(' Title: ')
}

function standardInputValueTemplate(result) {
    return result && [result.title].filter(element => element).join(' Title: ')
}

function inputValueTemplate(result) {
    return result && [result.name, result.ukprn].filter(element => element).join(' UKPRN: ')
}

function suggestionTemplate(result) {
    return result && [result.name, result.ukprn].filter(element => element).join(' UKPRN: ')
}

AutoComplete.prototype.autoComplete = function () {
    let that = this

    if (that.selectElement.id == "SearchTerm") {
        accessibleAutocomplete.enhanceSelectElement({
            selectElement: that.selectElement,
            minLength: 2,
            autoselect: false,
            defaultValue: '',
            displayMenu: 'overlay',
            placeholder: '',
            source: that.getSuggestions,
            showAllValues: false,
            confirmOnBlur: false,
            onConfirm: that.onConfirm,
            templates: {
                inputValue: inputValueTemplate,
                suggestion: suggestionTemplate
            }
        });
    }

    if (that.selectElement.id == "courseSearchTerm") {
        accessibleAutocomplete.enhanceSelectElement({
            selectElement: that.selectElement,
            minLength: 2,
            autoselect: false,
            defaultValue: '',
            displayMenu: 'overlay',
            placeholder: '',
            source: that.getStandards,
            showAllValues: false,
            confirmOnBlur: false,
            onConfirm: that.onStandardsConfirm,
            templates: {
                inputValue: standardInputValueTemplate,
                suggestion: standardTemplate
            }
        });
    }
    
}

function nodeListForEach(nodes, callback) {
    if (window.NodeList.prototype.forEach) {
        return nodes.forEach(callback)
    }
    for (let i = 0; i < nodes.length; i++) {
        callback.call(window, nodes[i], i, nodes);
    }
}

let autoCompletes = document.querySelectorAll('[data-module="autoComplete"]')

nodeListForEach(autoCompletes, function (autoComplete) {
    new AutoComplete(autoComplete).init()
})
