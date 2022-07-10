var req = async (url, data, method) => {
    var res = await fetch("api/" + url, {
        method: method,
        headers: {
            'Content-Type': 'application/json'
        }, 
        body: JSON.stringify(data)
    });

    var rtn;

    try {
        rtn = {status: res.status, json: await res.json()};
    } catch (e) {
        rtn = {status: res.status};
    }
    
    console.log(rtn);
    return rtn;
}

var req2 = async (url, method) => {
    var res = await fetch("api/" + url, {
        method: method,
        headers: {
            'Content-Type': 'application/json'
        }
    });

    var rtn = {status: res.status, json: await res.json()};
    console.log(rtn);
    return rtn;
}

var getReq = async (url) => {
    return await req2(url, "GET");
}

var postReq = async (url, data) => {
    return await req(url, data, "POST");
}

var deleteReq = async (url) => {
    return await req2(url, "DELETE");
}

var patchReq = async (url, data) => {
    return await req(url, data, "PATCH");
}


var spaceBetween = (str) => {
    var rtn = "";
    for (var i = 0; i < str.length; i++) {
        if (str[i] == str[i].toUpperCase()) {
            rtn += ' ';
        }

        rtn += str[i];
    }

    return rtn;
}


export {getReq, postReq, deleteReq, patchReq, spaceBetween}