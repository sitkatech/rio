const fs = require("fs");
const path = process.argv[2] + '/model';

// process.argv.forEach(function (val, index, array) {
//     console.log(index + ': ' + val);
//});

try {
    fs.rmdirSync(path, { recursive: true });
    console.log("Folder removed:", path);
} catch (err) {
    console.error(err);
}