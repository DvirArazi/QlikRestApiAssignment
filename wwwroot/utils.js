//POST request
//============
var postReq = async (url, data) => {
    return await (await fetch(url, {
        method: "POST",
        headers: {
            'Content-Type': 'application/json'
        }, 
        body: JSON.stringify(data)
    })).json();
}

var deleteReq = async (url, data) => {
    return await (await fetch(url, {
        method: "DELETE",
        headers: {
            'Content-Type': 'application/json'
        }, 
        body: JSON.stringify(data)
    })).json();
}

var patchReq = async (url, data) => {
    return await (await fetch(url, {
        method: "PATCH",
        headers: {
            'Content-Type': 'application/json'
        }, 
        body: JSON.stringify(data)
    })).json();
}

export {postReq, deleteReq, patchReq}