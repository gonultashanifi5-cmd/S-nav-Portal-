const workspace = document.getElementById("question-workspace");
const form = document.getElementById("question-form");
const controls = document.getElementById("question-controls");
const list = document.getElementById("question-list");
const statusMessage = document.getElementById("question-status");
const title = document.getElementById("question-form-title");
const cancel = document.getElementById("question-cancel");
const optionFields = [...form.querySelectorAll('input[name^="Options["]')];
const errorFields = [...form.querySelectorAll("[data-error-for]")];
const questionsUrl = workspace.dataset.questionsUrl;

function showStatus(message, isError = false) {
    statusMessage.textContent = message;
    statusMessage.classList.toggle("notice-error", isError);
}

function showErrors(errors = {}) {
    for (const field of errorFields) {
        const name = field.dataset.errorFor;
        const message = errors[name]?.[0] ?? "";
        field.textContent = message;
        const input = form.elements.namedItem(name);
        if (message) {
            input.setAttribute("aria-invalid", "true");
        } else {
            input.removeAttribute("aria-invalid");
        }
    }
}

function resetForm() {
    form.reset();
    form.elements.namedItem("Id").value = "0";
    showErrors();
    title.textContent = "Soru ekle";
    cancel.hidden = true;
}

async function request(url, options) {
    const response = await fetch(url, options);
    if (response.status === 401 || response.redirected) {
        window.location.assign(workspace.dataset.loginUrl);
        throw new Error("Oturum süreniz doldu. Yeniden giriş yapın.");
    }

    return response;
}

async function refreshList(message) {
    try {
        const response = await request(questionsUrl);
        if (!response.ok) {
            throw new Error();
        }

        list.innerHTML = await response.text();
        showStatus(message);
    } catch {
        showStatus(message + " Güncel listeyi görmek için sayfayı yenileyin.", true);
    }
}

async function run(action) {
    if (controls.disabled) {
        return;
    }

    showStatus("");
    controls.disabled = true;
    workspace.setAttribute("aria-busy", "true");
    let focusTarget;
    try {
        focusTarget = await action();
    } catch (error) {
        showStatus(error.message || "İşlem tamamlanamadı. Yeniden deneyin.", true);
    } finally {
        controls.disabled = false;
        workspace.removeAttribute("aria-busy");
        focusTarget?.focus();
    }
}

form.addEventListener("submit", event => {
    event.preventDefault();
    const body = new FormData(form);
    void run(async () => {
        showErrors();
        const response = await request(questionsUrl, { method: "POST", body });
        if (response.status === 400 && response.headers.get("Content-Type")?.includes("json")) {
            const result = await response.json();
            showErrors(result.errors);
            showStatus("Formdaki bilgileri kontrol edin.", true);
            return form.querySelector('[aria-invalid="true"]');
        }

        if (!response.ok) {
            throw new Error("Soru kaydedilemedi. Sayfayı yenileyip tekrar deneyin.");
        }

        resetForm();
        await refreshList("Soru kaydedildi.");
        return form.elements.namedItem("Text");
    });
});

list.addEventListener("click", event => {
    const button = event.target.closest("button[data-action]");
    if (!button || controls.disabled) {
        return;
    }

    const url = questionsUrl + "/" + button.dataset.id;
    if (button.dataset.action === "edit") {
        void run(async () => {
            const response = await request(url);
            if (!response.ok) {
                throw new Error("Soru açılamadı. Sayfayı yenileyip tekrar deneyin.");
            }

            const question = await response.json();
            resetForm();
            form.elements.namedItem("Id").value = question.id;
            form.elements.namedItem("Text").value = question.text;
            form.elements.namedItem("Points").value = question.points;
            form.elements.namedItem("CorrectOptionPosition").value = question.correctOptionPosition;
            optionFields.forEach((input, index) => { input.value = question.options[index]; });
            title.textContent = "Soruyu düzenle";
            cancel.hidden = false;
            return form.elements.namedItem("Text");
        });
        return;
    }

    if (window.confirm("Bu soru silinsin mi?")) {
        void run(async () => {
            const token = form.elements.namedItem("__RequestVerificationToken");
            const body = new FormData();
            body.append(token.name, token.value);
            const response = await request(url + "/delete", { method: "POST", body });
            if (!response.ok) {
                throw new Error("Soru silinemedi. Sayfayı yenileyip tekrar deneyin.");
            }

            if (form.elements.namedItem("Id").value === button.dataset.id) {
                resetForm();
            }

            await refreshList("Soru silindi.");
            return form.elements.namedItem("Text");
        });
    }
});

cancel.addEventListener("click", () => {
    if (!controls.disabled) {
        resetForm();
        showStatus("");
        form.elements.namedItem("Text").focus();
    }
});
