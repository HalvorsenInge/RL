
const { execSync } = require("child_process");

try {
  const out = execSync("cygpath -u 'C:\\Users'");
  console.log("OUTPUT:", out.toString());
} catch (e) {
  console.error("ERROR:", e.message);
}

