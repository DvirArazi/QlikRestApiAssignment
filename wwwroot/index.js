import { postReq, deleteReq, patchReq } from "./utils.js";

var submit = document.getElementById("submit");
var table = document.getElementById("table");
var textbox = document.getElementById("textbox");
var notice = document.getElementById("notice");

//Add Row
//=======
var addRow = (text, traits) => {
    var row = table.insertRow();

    var textbox = document.createElement("input");
    textbox.type = "text";
    textbox.value = text;

    var cell0 = row.insertCell();
    cell0.appendChild(textbox);

    console.log(traits);

    var traitCellList = {};
    for (const [key, value] of Object.entries(traits)) {
        traitCellList[key] = row.insertCell();
        traitCellList[key].innerHTML = value;
    }

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

    var onTextboxUpdate = async () => {
        if (text != textbox.value) {
            var res = await patchReq("update", {OldText: text, NewText: textbox.value});
            if (!res.isNewInDB) {
                text = textbox.value;
                for (const [key, value] of Object.entries(res.traits)) {
                    traitCellList[key].innerHTML = value;
                }
                notice.innerText = "";
            } else {
                textbox.value = text;
                notice.innerText = "The message is already in the database.";
            }
        }
    }
    textbox.onkeydown = (e) => {
        if (e.key == "Enter") {
            onTextboxUpdate();
        }
    };
    textbox.onblur = ()=>{
        onTextboxUpdate();
    };
}

//Build Table
//===========
(async () => {
    //Get All Messages from DB
    //========================
    var allRes = await (await fetch("all")).json();
    console.log(allRes);

    var row = table.insertRow();
    ["words"].concat(allRes.titles).forEach((title)=>{
        var tempCell = row.insertCell();
        tempCell.style = `
            text-align: center;
            padding: 5px;
        `;
        tempCell.innerHTML = `<b> ${title} </b>`;
    });

    //Add row after input of the top text input
    //=========================================
    var onTextInput = async () => {
        var addRes = await postReq("add", {Text: textbox.value});
        console.log(addRes);
        if (!addRes.isInDB) {
            addRow(textbox.value, addRes.traits);
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
    allRes.messages.forEach((message) => {
        addRow(message.Text, message.Traits)
    });
})();
