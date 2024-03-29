module.exports = {
  env: {
    browser: true,
    es6: true,
  },
  extends: [],
  globals: {
    Atomics: 'readonly',
    SharedArrayBuffer: 'readonly',
  },
  plugins: [
    "@typescript-eslint",
    "only-warn",
    "import",
    "unicorn",
  ],
  parser: "@typescript-eslint/parser",
  parserOptions: {
    project: './tsconfig.json',
    ecmaVersion: 2018,
    sourceType: 'module',
  },
  rules: {
    // Possible Errors
    'for-direction': "error",
    'getter-return': "error",
    'no-async-promise-executor': "error",
    'no-await-in-loop': "off",
    'no-compare-neg-zero': "error",
    'no-cond-assign': "error",
    'no-console': "off",
    'no-constant-condition': "error",
    'no-control-regex': "error",
    'no-debugger': "error",
    'no-dupe-args': "off", // Already illegal
    'no-dupe-keys': "off", // Already illegal
    'no-duplicate-case': "error",
    'no-empty': "error",
    'no-empty-character-class': "error",
    'no-ex-assign': "error",
    'no-extra-boolean-cast': "error",
    'no-extra-parens': 'off', // Typescript variant preferred
    'no-extra-semi': "error",
    'no-func-assign': "error",
    'no-inner-declarations': "error",
    'no-invalid-regexp': ["error", { "allowConstructorFlags": ["u", "y"] }],
    'no-irregular-whitespace': ["error", { "skipStrings": false }],
    'no-misleading-character-class': "error",
    'no-obj-calls': "error",
    'no-regex-spaces': "error",
    'no-sparse-arrays': "error",
    'no-template-curly-in-string': "error", // !!! Disable if template strings only allowed
    'no-unexpected-multiline': "error",
    'no-unreachable': "off", // Visible already
    'no-unsafe-finally': "error",
    'no-unsafe-negation': "off",
    'require-atomic-updates': "error",
    'use-isnan': "error",
    'valid-typeof': "off", // Already type checked

    // Best Practires
    'accessor-pairs': "error",
    'array-callback-return': 'off',
    'block-scoped-var': 'off',
    'class-methods-use-this': 'off',
    'complexity': 'off',
    'consistent-return': 'off', // Hopefully a typescript solution is found
    'curly': ["error", "multi-line", "consistent"],
    'default-case': 'off',
    'dot-location': 'off',
    'dot-notation': 'error',
    'eqeqeq': 'error',
    'guard-for-in': 'off', // Not a problem in modern javascript?
    'max-classes-per-file': 'off',
    'no-alert': 'off',
    'no-caller': 'error',
    'no-case-declarations': 'error',
    'no-div-regex': 'off',
    'no-else-return': 'off',
    'no-empty-function': ["error", { "allow": ["functions", "methods"] }],
    'no-empty-pattern': 'off',
    'no-eq-null': 'off', // Deprecated. '===' works
    'no-eval': 'error',
    'no-extend-native': 'off', // Not allowed in typescript
    'no-extra-bind': 'error',
    'no-extra-label': 'error',
    'no-fallthrough': 'error',
    'no-floating-decimal': 'error',
    'no-global-assign': 'off', // Already disallowed in typescript
    'no-implicit-coercion': 'error',
    'no-implicit-globals': 'off',
    'no-implied-eval': 'off',
    'no-invalid-this': 'off', // Handled by typescript
    'no-iterator': 'off',
    'no-labels': 'off',
    'no-lone-blocks': 'error',
    'no-loop-func': 'off',
    'no-magic-numbers': 'off',
    'no-multi-spaces': 'error',
    'no-multi-str': 'off',
    'no-new': 'error',
    'no-new-func': 'error',
    'no-new-wrappers': 'error',
    'no-octal': 'off', // Disallowed
    'no-octal-escape': 'off',
    'no-param-reassign': 'off',
    'no-proto': 'off', // Disallowed
    'no-redeclare': 'off', // Var is disabled and thus this can't happen
    'no-restricted-properties': 'off',
    'no-return-assign': 'error',
    'no-return-await': 'error',
    'no-script-url': 'off',
    'no-self-assign': 'error',
    'no-self-compare': 'error',
    'no-sequences': 'error',
    'no-throw-literal': 'error',
    'no-unmodified-loop-condition': 'error',
    'no-unused-expressions': 'off',
    'no-unused-labels': 'error',
    'no-useless-call': 'error',
    'no-useless-catch': 'error',
    'no-useless-concat': 'error',
    'no-useless-escape': 'off', // False positives?
    'no-useless-return': 'error',
    'no-void': 'off',
    'no-warning-comments': 'off',
    'no-with': 'off', // Already disallowed in typescript
    'prefer-named-capture-group': 'off',
    'prefer-promise-reject-errors': 'error',
    'radix': 'off',
    'require-await': 'off',
    'require-unicode-regexp': 'off',
    'vars-on-top': 'off',
    'wrap-iife': 'off',
    'yoda': 'error',

    // Strict
    'strict': ['error', "never"],

    // Variables
    'init-declarations': 'off',
    'no-delete-var': 'off', // Already disallowed
    'no-label-var': 'error',
    'no-restricted-globals': 'off',
    'no-shadow': 'off',
    'no-shadow-restricted-names': 'off', // Broken?
    'no-undef': 'off', // Already disallowed in typescript
    'no-undef-init': 'error',
    'no-undefined': 'off', // Already disallowed in typescript
    'no-unused-vars': 'off',
    'no-use-before-define': 'off',

    // Node.js and CommonJS
    'callback-return': 'off',
    'global-require': 'off',
    'handle-callback-err': 'off',
    'no-buffer-constructor': 'off',
    'no-mixed-requires': 'off',
    'no-new-require': 'off',
    'no-path-concat': 'off',
    'no-process-env': 'off',
    'no-process-exit': 'off',
    'no-restricted-modules': 'off',
    'no-sync': 'off',

    // Stylistic issues
    'array-bracket-newline': ["error", { "multiline": true }],
    'array-bracket-spacing': 'error',
    'array-element-newline': 'off',
    'block-spacing': 'error',
    'brace-style': ["error", "1tbs", { "allowSingleLine": true }],
    'camelcase': 'off',
    'capitalized-comments': 'off', // Flags variable names etc.
    'comma-dangle': ["error", "always-multiline"],
    'comma-spacing': 'error',
    'comma-style': 'error',
    'computed-property-spacing': 'error',
    'consistent-this': 'off',
    'eol-last': 'error',
    'func-call-spacing': 'off', // Typescript variant preferred
    'func-name-matching': 'off',
    'func-names': 'off',
    'func-style': 'off',
    'function-paren-newline': 'off',
    'id-blacklist': 'off',
    'id-length': 'off',
    'id-match': 'off',
    'implicit-arrow-linebreak': 'error',
    'indent': 'off', // Typescript variant preferred
    'jsx-quotes': 'off',
    'key-spacing': 'error',
    'keyword-spacing': 'error',
    'line-comment-position': 'off', // Too restrictive
    'linebreak-style': 'off',
    'lines-around-comment': 'off',
    'lines-between-class-members': 'off',
    'max-depth': 'off',
    'max-len': 'off',
    'max-lines': 'off',
    'max-lines-per-function': 'off',
    'max-nested-callbacks': 'off',
    'max-params': 'off',
    'max-statements': 'off',
    'max-statements-per-line': 'off',
    'multiline-comment-style': 'off',
    'multiline-ternary': ["error", "always-multiline"],
    'new-cap': 'off', // Typescript variant preferred
    'new-parens': 'error',
    'newline-per-chained-call': 'off', // Low quality
    'no-array-constructor': 'off', // Typescript variant preferred
    'no-bitwise': 'off', // ~~ is way too useful
    'no-continue': 'off',
    'no-inline-comments': 'off',
    'no-lonely-if': 'error',
    'no-mixed-operators': ["error", { "groups": [["&&", "||"]] }],
    'no-mixed-spaces-and-tabs': "error",
    'no-multi-assign': 'error',
    'no-multiple-empty-lines': ["error", { "max": 2, "maxEOF": 0 }],
    'no-negated-condition': 'error',
    'no-nested-ternary': 'off',
    'no-new-object': 'error',
    'no-plusplus': 'off',
    'no-restricted-syntax': 'off',
    'no-tabs': "error",
    'no-ternary': 'off',
    'no-trailing-spaces': ["error", { "ignoreComments": true }],
    'no-underscore-dangle': 'off',
    'no-unneeded-ternary': ["error", { "defaultAssignment": false }],
    'no-whitespace-before-property': 'error',
    'nonblock-statement-body-position': ["error", "beside"],
    'object-curly-newline': ["error", { "multiline": true, "consistent": true }],
    'object-curly-spacing': ["error", "always", { "objectsInObjects": true }],
    'object-property-newline': 'off',
    'one-var': ["error", "never"],
    'one-var-declaration-per-line': ["error", "always"],
    'operator-assignment': 'error',
    'operator-linebreak': ["error", "after"],
    'padded-blocks': ["error", "never"],
    'padding-line-between-statements': [
      "error",
      { "blankLine": "always", "prev": "*", "next": ["const", "let"] },
      { "blankLine": "any", "prev": ["const", "let", "var"], "next": ["const", "let"] }
    ],
    'prefer-object-spread': 'error',
    'quote-props': ["error", "consistent-as-needed"],
    'quotes': ["error", "single", { "allowTemplateLiterals": false }],
    'semi': ['error', "never"],
    'semi-spacing': 'off', // Semicolons are not used
    'semi-style': ["error", "first"],
    'sort-keys': 'off',
    'sort-vars': 'off',
    'space-before-blocks': "error",
    'space-before-function-paren': ["error", {
      "anonymous": "never",
      "named": "never",
      "asyncArrow": "always"
    }],
    'space-in-parens': 'error',
    'space-infix-ops': 'error',
    'space-unary-ops': 'error',
    'spaced-comment': 'error',
    'switch-colon-spacing': 'error',
    'template-tag-spacing': 'off',
    'unicode-bom': 'off',
    'wrap-regex': 'off',

    // ECMAScript 6
    'arrow-body-style': ["error", "as-needed", { "requireReturnForObjectLiteral": true }],
    'arrow-parens': [2, "as-needed", { "requireForBlockBody": true }],
    'arrow-spacing': 'error',
    'constructor-super': 'off', // Already checked by typescript
    'generator-star-spacing': ["error", { "before": false, "after": true }],
    'no-class-assign': 'off', // Handled well enough by typescript
    'no-confusing-arrow': 'off',
    'no-const-assign': 'off', // Typescript disallows
    'no-dupe-class-members': 'off', // Typescript disallows
    'no-duplicate-imports': 'off', // fspromises problems
    'no-new-symbol': 'error',
    'no-restricted-imports': 'off',
    'no-this-before-super': 'off', // Typescript disallows
    'no-useless-computed-key': 'error',
    'no-useless-constructor': 'off', // Handled by other rules
    'no-useless-rename': 'error',
    'no-var': 'error',
    'object-shorthand': 'error',
    'prefer-arrow-callback': 'error',
    'prefer-const': 'error',
    'prefer-destructuring': 'off',
    'prefer-numeric-literals': 'off',
    'prefer-rest-params': 'error',
    'prefer-spread': 'off', // Don't quite use apply enough
    'prefer-template': 'error',
    'require-yield': 'off',
    'rest-spread-spacing': 'error',
    'sort-imports': 'off', // Dedicated plugin with autofix
    'symbol-description': 'error',
    'template-curly-spacing': 'error',
    'yield-star-spacing': 'error',


    // @typescript-eslint // !!!
    '@typescript-eslint/adjacent-overload-signatures': 'error',
    '@typescript-eslint/array-type': ['error', { default: 'array-simple' }],
    '@typescript-eslint/await-thenable': 'error',
    '@typescript-eslint/ban-ts-ignore': 'off',
    '@typescript-eslint/ban-types': 'error',
    '@typescript-eslint/camelcase': ["error", { 'properties': "never" }],
    '@typescript-eslint/class-name-casing': 'error',
    '@typescript-eslint/explicit-function-return-type': 'off',
    '@typescript-eslint/explicit-member-accessibility': ['error', {
      "accessibility": "explicit",
      overrides: {
        accessors: 'explicit',
        constructors: 'off',
        methods: 'explicit',
        properties: 'explicit',
        parameterProperties: 'explicit'
      }
    }],
    '@typescript-eslint/func-call-spacing': 'error',
    '@typescript-eslint/generic-type-naming': 'off', // Type argument rule
    '@typescript-eslint/indent': [
      "error", 2, { // Tabs are not well supported
        "SwitchCase": 1,
        "VariableDeclarator": "first"
      }],
    '@typescript-eslint/interface-name-prefix': ['error', 'never'],
    '@typescript-eslint/member-delimiter-style': ['error', { "multiline": { "delimiter": "none", }, "singleline": { "delimiter": "comma", } }],
    '@typescript-eslint/member-naming': 'off',
    '@typescript-eslint/member-ordering': ['error', { // Aids remove
      "default": [
        "public-static-field",
        "protected-static-field",
        "private-static-field",
        "public-instance-field",
        "protected-instance-field",
        "private-instance-field",
        "public-field",
        "protected-field",
        "private-field",
        "static-field",
        "instance-field",
        "constructor",
        "public-static-method",
        "protected-static-method",
        "private-static-method",
        "public-instance-method",
        "protected-instance-method",
        "private-instance-method",
        "public-method",
        "protected-method",
        "private-method",
        "static-method",
        "instance-method",
        "method", // fix index types
        "field", // fix index types
      ],
    }],
    '@typescript-eslint/no-array-constructor': 'error',
    '@typescript-eslint/no-empty-interface': 'off',
    '@typescript-eslint/no-explicit-any': 'off', // Blocks typing as any
    '@typescript-eslint/no-extra-parens': ['error', 'all', { "nestedBinaryExpressions": false }],
    '@typescript-eslint/no-extraneous-class': 'off',
    '@typescript-eslint/no-floating-promises': 'off', // Not implemented atm
    '@typescript-eslint/no-for-in-array': 'error',
    '@typescript-eslint/no-inferrable-types': 'off',
    '@typescript-eslint/no-magic-numbers': 'off',
    '@typescript-eslint/no-misused-new': 'off',
    '@typescript-eslint/no-namespace': 'off',
    '@typescript-eslint/no-non-null-assertion': 'off',
    '@typescript-eslint/no-object-literal-type-assertion': 'off',
    '@typescript-eslint/no-parameter-properties': 'off',
    '@typescript-eslint/no-require-imports': 'error',
    '@typescript-eslint/no-this-alias': 'error',
    '@typescript-eslint/no-triple-slash-reference': 'off',
    '@typescript-eslint/no-type-alias': 'off',
    '@typescript-eslint/no-unnecessary-qualifier': 'off',
    '@typescript-eslint/no-unnecessary-type-assertion': 'error',
    '@typescript-eslint/no-unused-vars': 'off',
    '@typescript-eslint/no-use-before-define': 'off',
    '@typescript-eslint/no-useless-constructor': 'off',
    '@typescript-eslint/no-var-requires': 'off',
    '@typescript-eslint/prefer-for-of': 'off', // Unicorn variant preferred
    '@typescript-eslint/prefer-function-type': 'off',
    '@typescript-eslint/prefer-includes': 'off',
    '@typescript-eslint/prefer-namespace-keyword': 'off',
    '@typescript-eslint/prefer-regexp-exec': 'off',
    '@typescript-eslint/prefer-string-starts-ends-with': 'error',
    '@typescript-eslint/promise-function-async': 'off',
    '@typescript-eslint/require-array-sort-compare': 'off',
    '@typescript-eslint/restrict-plus-operands': 'off',
    '@typescript-eslint/restrict-template-expressions': ['off', { // Too annyoing
      allowNumber: true,
      allowBoolean: true,
      allowNullable: true,
    }],
    '@typescript-eslint/semi': 'off',
    '@typescript-eslint/type-annotation-spacing': 'error',
    '@typescript-eslint/unbound-method': 'off', // False positives?
    '@typescript-eslint/unified-signatures': 'off', // Too aggressive


    // import
    'import/no-unresolved': 'off',
    'import/named': 'off',
    'import/default': 'off',
    'import/namespace': 'off',
    'import/no-restricted-paths': 'off',
    'import/no-absolute-path': 'off',
    'import/no-dynamic-require': 'off',
    'import/no-internal-modules': 'off',
    'import/no-webpack-loader-syntax': 'off',
    'import/no-self-import': 'off',
    'import/no-cycle': 'off',
    'import/no-useless-path-segments': 'off',
    'import/no-relative-parent-imports': 'off',
    'import/no-unused-modules': 'off',
    'import/export': 'off',
    'import/no-named-as-default': 'off',
    'import/no-named-as-default-member': 'off',
    'import/no-deprecated': 'off',
    'import/no-extraneous-dependencies': 'off',
    'import/no-mutable-exports': 'off',
    'import/no-unused-modules': 'off',
    'import/unambiguous': 'off',
    'import/no-commonjs': 'off',
    'import/no-amd': 'off',
    'import/no-nodejs-modules': 'off',
    'import/first': 'error',
    'import/exports-last': 'off',
    'import/no-duplicates': 'off',
    'import/no-namespace': 'off',
    'import/extensions': 'off',
    'import/order': ["error", { "newlines-between": "always" }],
    'import/newline-after-import': 'off',
    'import/prefer-default-export': 'off',
    'import/max-dependencies': 'off',
    'import/no-unassigned-import': 'off',
    'import/no-named-default': 'off',
    'import/no-default-export': 'off',
    'import/no-named-export': 'off',
    'import/no-anonymous-default-export': 'off',
    'import/group-exports': 'off',
    'import/dynamic-import-chunkname': 'off',


    // Unicorn
    "unicorn/catch-error-name": "off",
    "unicorn/custom-error-definition": "off",
    "unicorn/error-message": "off",
    "unicorn/escape-case": "off",
    "unicorn/explicit-length-check": "off",
    "unicorn/filename-case": "off",
    "unicorn/import-index": "off",
    "unicorn/new-for-builtins": "off",
    "unicorn/no-abusive-eslint-disable": "off",
    "unicorn/no-array-instanceof": "error",
    "unicorn/no-console-spaces": "off",
    "unicorn/no-fn-reference-in-iterator": "off",
    "unicorn/no-for-loop": "off",
    "unicorn/no-hex-escape": "off",
    "unicorn/no-new-buffer": "error",
    "unicorn/no-process-exit": "off",
    "unicorn/no-unreadable-array-destructuring": "off",
    "unicorn/no-unsafe-regex": "off", // Too aggressive?
    "unicorn/no-unused-properties": "off",
    "unicorn/no-zero-fractions": "off",
    "unicorn/number-literal-case": "off",
    "unicorn/prefer-add-event-listener": "off",
    "unicorn/prefer-event-key": "off",
    "unicorn/prefer-exponentiation-operator": "off",
    "unicorn/prefer-flat-map": "off",
    "unicorn/prefer-includes": "error",
    "unicorn/prefer-node-append": "off",
    "unicorn/prefer-node-remove": "off",
    "unicorn/prefer-query-selector": "off",
    "unicorn/prefer-spread": "error",
    "unicorn/prefer-starts-ends-with": "off",
    "unicorn/prefer-text-content": "off",
    "unicorn/prefer-type-error": "error",
    "unicorn/prevent-abbreviations": "off",
    "unicorn/regex-shorthand": "error",
    "unicorn/throw-new-error": "error",
  }
}
