export default {
    printWidth: 120,
    tabWidth: 4,
    useTabs: false,
    semi: true,
    singleQuote: false,
    trailingComma: "none",
    endOfLine: "crlf",
    overrides: [
        {
            files: "generated/**/*.ts",
            options: {
                requirePragma: true
            }
        }
    ]
};
