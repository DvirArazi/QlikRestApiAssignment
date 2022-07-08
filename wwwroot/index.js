import { postReq, deleteReq } from "./utils.js";

var submit = document.getElementById("submit");
var table = document.getElementById("table");
var textbox = document.getElementById("textbox");
var notice = document.getElementById("notice");

//Add Row
//=======
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
        var res = await deleteReq("delete", { Text: text });
        console.log(res);
        if (res.wasFound) {
            row.remove();
        }
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

//Build Table
//===========
(async () => {
    //Get All Messages from DB
    //========================
    var messages = await (await fetch("all")).json();
    console.log(messages);

    //Add row after input of the top text input
    //=========================================
    var onTextInput = async () => {
        var res = await postReq("add", {Text: textbox.value});
        console.log(res);
        if (!res.isInDB) {
            addRow(textbox.value, res.isPalindrome);
            notice.innerText = "";
        } else {
            notice.innerText = "The message is already in the database.";
        }
    }
    textbox.onkeydown = (e) => {
        if (e.key == "Enter") {
            onTextInput();
        }
    }
    submit.onclick = ()=>{
        onTextInput();
    };

    //Add row for each message
    //========================
    messages.forEach((message) => {
        addRow(message.Text, message.IsPalindrome)
    });
})();
