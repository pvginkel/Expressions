grammar Flee;

options
{
	language = CSharp3;
	TokenLabelType = CommonToken;
	output = AST;
	ASTLabelType = CommonTree;
}

@header
{
	using Expressions.Ast;
}

@lexer::namespace { Expressions.Flee }
@parser::namespace { Expressions.Flee }
@modifier { internal }

prog returns [IAstNode value]
	: expression { $value = $expression.value; }
	;

expression returns [IAstNode value]
	: e=logical_xor_expression { $value = $e.value; }
		(
			OR e=logical_xor_expression
			{ $value = new BinaryExpression($value, $e.value, ExpressionType.Xor); }
		)*
	;

logical_xor_expression returns [IAstNode value]
	: e=logical_and_expression { $value = $e.value; }
		(
			XOR e=logical_and_expression
			{ $value = new BinaryExpression($value, $e.value, ExpressionType.Or); }
		)*
	;

logical_and_expression returns [IAstNode value]
	: e=equality_expression { $value = $e.value; }
		( AND e=equality_expression
		  { $value = new BinaryExpression($value, $e.value, ExpressionType.Or); } )*
	;

equality_expression returns [IAstNode value]
	: e=in_expression { $value = $e.value; }
		(
			( '=' e=in_expression { $value = new BinaryExpression($value, $e.value, ExpressionType.Equals); }
			| '<>' e=in_expression { $value = new BinaryExpression($value, $e.value, ExpressionType.NotEquals); }
			)
		)*
	;

in_expression returns [IAstNode value]
	: e=relational_expression { $value = $e.value; }
		(
			IN
				( IDENTIFIER { $value = new BinaryExpression($value, CreateIdentifier($IDENTIFIER.text), ExpressionType.In); }
				| '(' l=argument_expression_list ')' { $value = new BinaryExpression($value, $l.value, ExpressionType.In); }
				)
		)?
	;

relational_expression returns [IAstNode value]
	: e=shift_expression { $value = $e.value; }
		(
			( '<' e=shift_expression { $value = new BinaryExpression($value, $e.value, ExpressionType.Less); }
			| '>' e=shift_expression { $value = new BinaryExpression($value, $e.value, ExpressionType.Greater); }
			| '<=' e=shift_expression { $value = new BinaryExpression($value, $e.value, ExpressionType.LessOrEquals); }
			| '>=' e=shift_expression { $value = new BinaryExpression($value, $e.value, ExpressionType.GreaterOrEquals); }
			)
		)*
	;

shift_expression returns [IAstNode value]
	: e=additive_expression { $value = $e.value; }
		(
			( '<<' e=additive_expression { $value = new BinaryExpression($value, $e.value, ExpressionType.ShiftLeft); }
			| '>>' e=additive_expression { $value = new BinaryExpression($value, $e.value, ExpressionType.ShiftRight); }
			)
		)*
	;

additive_expression returns [IAstNode value]
	: e=multiplicative_expression { $value = $e.value; }
		( '+' e=multiplicative_expression { $value = new BinaryExpression($value, $e.value, ExpressionType.Add); }
		| '-' e=multiplicative_expression { $value = new BinaryExpression($value, $e.value, ExpressionType.Subtract); }
		)*
	;

multiplicative_expression returns [IAstNode value]
	: e=cast_expression { $value = $e.value; }
		( '^' e=cast_expression { $value = new BinaryExpression($value, $e.value, ExpressionType.Power); }
		| '*' e=cast_expression { $value = new BinaryExpression($value, $e.value, ExpressionType.Multiply); }
		| '/' e=cast_expression { $value = new BinaryExpression($value, $e.value, ExpressionType.Divide); }
		| '%' e=cast_expression { $value = new BinaryExpression($value, $e.value, ExpressionType.Modulo); }
		)*
	;

cast_expression returns [IAstNode value]
	: CAST '(' e=expression ',' t=type_expression ')' { $value = new Cast($e.value, $t.value); }
	| u=unary_expression { $value = $u.value; }
	;

type_expression returns [TypeIdentifier value]
	: IDENTIFIER { $value = new TypeIdentifier($IDENTIFIER.text, 0); }
		(
			'[' { $value = new TypeIdentifier($value.Name, 1); }
				( ',' { $value = new TypeIdentifier($value.Name, $value.ArrayIndex + 1); }
				)*
			']'
		)?
	;

unary_expression returns [IAstNode value]
	: p=postfix_expression { $value = $p.value; }
	|
		( '+' e=cast_expression { $value = new UnaryExpression($e.value, ExpressionType.Plus); }
		| '-' e=cast_expression { $value = new UnaryExpression($e.value, ExpressionType.Minus); }
		| NOT e=cast_expression { $value = new UnaryExpression($e.value, ExpressionType.Not); }
		)
	;

argument_expression_list returns [AstNodeCollection value]
	: e=expression { $value = new AstNodeCollection($e.value); }
		( ',' e=expression { $value = new AstNodeCollection($value, $e.value); }
		)*
	;

postfix_expression returns [IAstNode value]
	: p=primary_expression { $value = $p.value; }
		( '[' e=argument_expression_list ']' { $value = new Index($value, $e.value); }
        | '(' ')' { $value = new MethodCall($value); }
        | '(' e=argument_expression_list ')' { $value = new MethodCall($value, $e.value); }
        | '.' IDENTIFIER { $value = new MemberAccess($value, $IDENTIFIER.text); }
        )*
	;

primary_expression returns [IAstNode value]
	: IDENTIFIER { $value = CreateIdentifier($IDENTIFIER.text); }
	| constant { $value = $constant.value; }
	| '(' expression ')' { $value = $expression.value; }
	;

constant returns [Constant value]
    : TRUE { $value = Constant.True; }
	| FALSE { $value = Constant.False; }
	| NULL { $value = Constant.Null; }
	| DATETIME_LITERAL { $value = ParseDateTime($DATETIME_LITERAL.text); }
	| TIMESPAN_LITERAL { $value = ParseTimeSpan($TIMESPAN_LITERAL.text); }
    | HEX_LITERAL { $value = ParseHex($HEX_LITERAL.text); }
    | DECIMAL_LITERAL { $value = ParseDecimal($DECIMAL_LITERAL.text); }
    | CHARACTER_LITERAL { $value = ParseCharacter($CHARACTER_LITERAL.text); }
	| STRING_LITERAL { $value = ParseString($STRING_LITERAL.text); }
    | FLOATING_POINT_LITERAL { $value = ParseFloatingPoint($FLOATING_POINT_LITERAL.text); }
    ;

AND
	: ('A'|'a')('N'|'n')('D'|'d')
	;
	
IN
	: ('I'|'i')('N'|'n')
	;
	
OR
	: ('O'|'o')('R'|'r')
	;
	
XOR
	: ('X'|'x')('O'|'o')('R'|'r')
	;
	
CAST
	: ('C'|'c')('A'|'a')('S'|'s')('T'|'t')
	;
	
NOT
	: ('N'|'n')('O'|'o')('T'|'t')
	;

TRUE
	: ('T'|'t')('R'|'r')('U'|'u')('E'|'e')
	;

FALSE
	: ('F'|'f')('A'|'a')('L'|'l')('S'|'s')('E'|'e')
	;

NULL
	: ('N'|'n')('U'|'u')('L'|'l')('L'|'l')
	;

CHARACTER_LITERAL
    : '\'' ( ~('\\'|'\'') | EscapeSequence ) '\''
    ;

TIMESPAN_LITERAL
	: '#' '#' ( ~'#' )* '#'
	;

DATETIME_LITERAL
	: '#' ( ~'#' )* '#'
	;

STRING_LITERAL
	:  '"' ( ~('\\'|'"') | EscapeSequence )* '"'
    ;

HEX_LITERAL
	: '0' ('x'|'X') HexDigit+ IntegerTypeSuffix?
	;

DECIMAL_LITERAL
	: ('0' | '1'..'9' '0'..'9'*) IntegerTypeSuffix?
	;

fragment
HexDigit : ('0'..'9'|'a'..'f'|'A'..'F') ;

fragment
IntegerTypeSuffix
	: ('u' | 'U') ('l' | 'L')?
	| ('l' | 'L')
	;

FLOATING_POINT_LITERAL
    : ('0'..'9')+ '.' ('0'..'9')* Exponent? FloatTypeSuffix?
    | '.' ('0'..'9')+ Exponent? FloatTypeSuffix?
    | ('0'..'9')+ Exponent FloatTypeSuffix?
    | ('0'..'9')+ FloatTypeSuffix
	;

fragment
Exponent
	: ('e'|'E') ('+'|'-')? ('0'..'9')+
	;

fragment
FloatTypeSuffix
	: ('f'|'F'|'d'|'D'|'m'|'M')
	;

fragment
EscapeSequence
    : '\\' ('B'|'b'|'T'|'t'|'N'|'n'|'F'|'f'|'R'|'r'|'\"'|'\''|'\\')
    | UnicodeEscape
    ;

fragment
UnicodeEscape
	: '\\' ('u'|'U') HexDigit HexDigit HexDigit HexDigit
	;

IDENTIFIER
	: LETTER (LETTER|'0'..'9')*
	;
	
fragment
LETTER
	: 'A'..'Z'
	| 'a'..'z'
	| '_'
	;

WS
	: (' '|'\t'|'\r'|'\n')+
	{
// This construct is to make ANTLRWorks happy.
#if true
		Skip();
#else
		$channel = HIDDEN;
#endif
	}
	;
