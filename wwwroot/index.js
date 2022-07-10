import { getReq, postReq, deleteReq, patchReq, spaceBetween } from "./utils.js";

var submit = document.getElementById("submit");
var table = document.getElementById("table");
var textbox = document.getElementById("textbox");
var notice = document.getElementById("notice");

//Add Row
//=======
var addRow = (text, message) => {
    var row = table.insertRow();

    var textbox = document.createElement("input");
    textbox.type = "text";
    textbox.value = text;

    var cell0 = row.insertCell();
    cell0.appendChild(textbox);

    var traitCellList = {};
    for (const [key, value] of Object.entries(message.Traits)) {
        traitCellList[key] = row.insertCell();
        traitCellList[key].innerHTML = value;
    }

    var deleteBtn = document.createElement("button");
    deleteBtn.innerHTML = "Delete";
    deleteBtn.onclick = async () => {
        console.log(message.Id);
        var res = await deleteReq(`messages/${message.Id}`);
        if (res.status == 200) {
            row.remove();
        }
    };

    var cell2 = row.insertCell();
    cell2.appendChild(deleteBtn);

    var onTextboxUpdate = async () => {
        if (text != textbox.value) {
            var res = await patchReq(`messages/${message.Id}`, {OldText: text, NewText: textbox.value});
            if (res.status == 200) {
                text = textbox.value;
                for (const [key, value] of Object.entries(res.json.Traits)) {
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
    // var bla = await getReq("messages/62c8b686b04bffceb5b1e0de");
    // console.log(bla);
    // bla = await deleteReq("messages/62c8b686b04bffceb5b1e0de");

    //Get All Messages from DB
    //========================
    var allRes = await getReq("messages", {});

    if (allRes.json.length > 0) {
        var row = table.insertRow();
        ["words"].concat(Object.keys(allRes.json[0].Traits)).forEach((title)=>{
            var tempCell = row.insertCell();
            tempCell.style = `
                text-align: center;
                padding: 5px;
            `;
            tempCell.innerHTML = `<b>${spaceBetween(title)}</b>`;
        });
    }

    //Add row after input of the top text input
    //=========================================
    var onTextInput = async () => {
        var addRes = await postReq("messages", {Text: textbox.value});
        if (addRes.status == 200) {
            addRow(textbox.value, addRes.json);
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
    allRes.json.forEach((message) => {
        addRow(message.Text, message)
    });
})();
