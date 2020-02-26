# DESCRIPTION

The program using regex builds a tree that corresponds to the entered expression and produces for it

- a table of names;
- non-optimized code;
- optimized code.

The input string has the form: VARIABLE = EXPRESSION

The expression may include:

- signs of addition and multiplication ("+" and "*");
- parentheses ("(" and ")");
- constants (for example, 6; 4.12; 9e + 11, 7.14E-10);
- names of variables (sequences of letters and numbers starting with a letter)
