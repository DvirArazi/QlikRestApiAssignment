
var submit = document.getElementById("submit");
var table = document.getElementById("table");
var text = document.getElementById("text");
var notice = document.getElementById("notice");

var addRow = (text, isPalindrome) => {
    var row = table.insertRow();

    var textbox = document.createElement("input");
    textbox.type = "text";
    textbox.value = text;

    var cell0 = row.insertCell();
    cell0.appendChild(textbox);

    var cell1 = row.insertCell();
    cell1.innerHTML = isPalindrome;

    var deleteBtn = document.createElement("button");
    deleteBtn.innerHTML = "Delete";
    deleteBtn.onclick = async () => {
        await fetch(`delete/${text}`);
        row.remove();
    };

    var cell2 = row.insertCell();
    cell2.appendChild(deleteBtn);

    var onTextboxInput = async () => {
        if (text != textbox.value) {
            var res = await (await fetch(`update/${text}/${textbox.value}`)).json();
            if (!res.isInDB) {
                text = textbox.value;
                cell1.innerHTML = res.isPalindrome;
                notice.innerText = "";
            } else {
                textbox.value = text;
                notice.innerText = "The message is already in the database.";
            }
        }
    }
    textbox.onkeydown = (e) => {
        if (e.key == "Enter") {
            onTextboxInput();
        }
    };
    textbox.onblur = ()=>{
        onTextboxInput();
    };
}

(async () => {
    var messages = await (await fetch("all")).json();
    console.log(messages);

    var onTextInput = async () => {
        var res = await (await fetch(`add/${text.value}`)).json();
        if (!res.isInDB) {
            addRow(text.value, res.isPalindrome);
            notice.innerText = "";
        } else {
            notice.innerText = "The message is already in the database.";
        }
    }
    text.onkeydown = (e) => {
        if (e.key == "Enter") {
            onTextInput();
        }
    }
    submit.onclick = ()=>{
        onTextInput();
    };

    messages.forEach((message) => {
        addRow(message.Text, message.IsPalindrome)
    });
})();
