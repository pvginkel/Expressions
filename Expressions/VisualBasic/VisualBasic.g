grammar VisualBasic;

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

@rulecatch
{
    catch (RecognitionException) 
    {
        throw;
    }
}

@lexer::namespace { Expressions.VisualBasic }
@parser::namespace { Expressions.VisualBasic }
@lexer::modifier { internal }
@parser::modifier { internal }

prog returns [IAstNode value]
	: expression { $value = $expression.value; } EOF
	;

expression returns [IAstNode value]
	: e=logical_xor_expression { $value = $e.value; }
		( OR e=logical_xor_expression
		  { $value = new BinaryExpression($value, $e.value, ExpressionType.OrBoth); }
		| ORELSE e=logical_xor_expression
		  { $value = new BinaryExpression($value, $e.value, ExpressionType.Or); }
		)*
	;

logical_xor_expression returns [IAstNode value]
	: e=logical_and_expression { $value = $e.value; }
		( XOR e=logical_and_expression
		  { $value = new BinaryExpression($value, $e.value, ExpressionType.Xor); }
		)*
	;

logical_and_expression returns [IAstNode value]
	: e=unary_not_expression { $value = $e.value; }
		( AND e=unary_not_expression
		  { $value = new BinaryExpression($value, $e.value, ExpressionType.AndBoth); }
		| ANDALSO e=unary_not_expression
		  { $value = new BinaryExpression($value, $e.value, ExpressionType.And); }
		)*
	;

unary_not_expression returns [IAstNode value]
	: e=equality_expression { $value = $e.value; }
	| NOT u=unary_not_expression { $value = new UnaryExpression($u.value, ExpressionType.Not); }
	;

equality_expression returns [IAstNode value]
	: e=relational_expression { $value = $e.value; }
		(
			( '=' e=relational_expression { $value = new BinaryExpression($value, $e.value, ExpressionType.Compares); }
			| '<>' e=relational_expression { $value = new BinaryExpression($value, $e.value, ExpressionType.NotCompares); }
			| IS e=relational_expression { $value = new BinaryExpression($value, $e.value, ExpressionType.Equals); }
			)
		)*
	;

relational_expression returns [IAstNode value]
	: e=additive_expression { $value = $e.value; }
		(
			( '<' e=additive_expression { $value = new BinaryExpression($value, $e.value, ExpressionType.Less); }
			| '>' e=additive_expression { $value = new BinaryExpression($value, $e.value, ExpressionType.Greater); }
			| '<=' e=additive_expression { $value = new BinaryExpression($value, $e.value, ExpressionType.LessOrEquals); }
			| '>=' e=additive_expression { $value = new BinaryExpression($value, $e.value, ExpressionType.GreaterOrEquals); }
			)
		)*
	;

additive_expression returns [IAstNode value]
	: e=multiplicative_expression { $value = $e.value; }
		( '+' e=multiplicative_expression { $value = new BinaryExpression($value, $e.value, ExpressionType.Add); }
		| '-' e=multiplicative_expression { $value = new BinaryExpression($value, $e.value, ExpressionType.Subtract); }
		| '&' e=multiplicative_expression { $value = new BinaryExpression($value, $e.value, ExpressionType.Concat); }
		)*
	;

multiplicative_expression returns [IAstNode value]
	: e=power_expression { $value = $e.value; }
		( '*' e=power_expression { $value = new BinaryExpression($value, $e.value, ExpressionType.Multiply); }
		| '/' e=power_expression { $value = new BinaryExpression($value, $e.value, ExpressionType.Divide); }
		| '%' e=power_expression { $value = new BinaryExpression($value, $e.value, ExpressionType.Modulo); }
		)*
	;

power_expression returns [IAstNode value]
	: e=cast_expression { $value = $e.value; }
		( '^' e=cast_expression { $value = new BinaryExpression($value, $e.value, ExpressionType.Power); }
		)*
	;

cast_expression returns [IAstNode value]
	: CTYPE '(' e=expression ',' t=type_expression ')' { $value = new Cast($e.value, $t.value, CastType.Convert); }
	| DIRECTCAST '(' e=expression ',' t=type_expression ')' { $value = new Cast($e.value, $t.value, CastType.Cast); }
	| u=if_expression { $value = $u.value; }
	;

type_expression returns [TypeIdentifier value]
	: e=type_identifier { $value = new TypeIdentifier($e.value, 0); }
		(
			'(' { $value = new TypeIdentifier($value.Name, 1); }
				( ',' { $value = new TypeIdentifier($value.Name, $value.ArrayIndex + 1); }
				)*
			')'
		)?
	;

type_identifier returns [string value]
	: e=IDENTIFIER { $value = $e.text; } ( DOT e=IDENTIFIER { $value = $value + "." + $e.text; } )*
	;

if_expression returns [IAstNode value]
	: IIF '(' e=expression ',' t=expression ',' l=expression ')' { $value = new Conditional($e.value, $t.value, $l.value); }
	| u=unary_expression { $value = $u.value; }
	;

unary_expression returns [IAstNode value]
	: p=postfix_expression { $value = $p.value; }
	|
		( '+' e=cast_expression { $value = new UnaryExpression($e.value, ExpressionType.Plus); }
		| '-' e=cast_expression { $value = new UnaryExpression($e.value, ExpressionType.Minus); }
		)
	;

argument_expression_list returns [AstNodeCollection value]
	: e=expression { $value = new AstNodeCollection($e.value); }
		( ',' e=expression { $value = new AstNodeCollection($value, $e.value); }
		)*
	;

postfix_expression returns [IAstNode value]
	: p=primary_expression { $value = $p.value; }
		( '(' ')' { $value = new MethodCall($value); }
		| '(' e=argument_expression_list ')' { $value = new MethodCall($value, $e.value); }
		| DOT IDENTIFIER { $value = new MemberAccess($value, $IDENTIFIER.text); }
		)*
	;

primary_expression returns [IAstNode value]
	: IDENTIFIER { $value = CreateIdentifier($IDENTIFIER.text); }
	| constant { $value = $constant.value; }
	| '(' expression ')' { $value = new UnaryExpression($expression.value, ExpressionType.Group); }
	;

constant returns [Constant value]
	: TRUE { $value = Constant.True; }
	| FALSE { $value = Constant.False; }
	| NOTHING { $value = Constant.Null; }
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

ANDALSO
	: ('A'|'a')('N'|'n')('D'|'d')('A'|'a')('L'|'l')('S'|'s')('O'|'o')
	;

OR
	: ('O'|'o')('R'|'r')
	;

ORELSE
	: ('O'|'o')('R'|'r')('E'|'e')('L'|'l')('S'|'s')('E'|'e')
	;

XOR
	: ('X'|'x')('O'|'o')('R'|'r')
	;

CTYPE
	: ('C'|'c')('T'|'t')('Y'|'y')('P'|'p')('E'|'e')
	;
	
DIRECTCAST
	: ('D'|'d')('I'|'i')('R'|'r')('E'|'e')('C'|'c')('T'|'t')('C'|'c')('A'|'a')('S'|'s')('T'|'t')
	;

IIF
	: ('I'|'i')('I'|'i')('F'|'f')
	;

IS
	: ('I'|'i')('S'|'s')
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

NOTHING
	: ('N'|'n')('O'|'o')('T'|'t')('H'|'h')('I'|'i')('N'|'n')('G'|'g')
	;

DOT
	: // '.'
	;

CHARACTER_LITERAL
	: '\'' ~'\'' '\''
	;

TIMESPAN_LITERAL
	: '#' '#' ( ~'#' )* '#'
	;

DATETIME_LITERAL
	: '#' ( ~'#' )* '#'
	;

STRING_LITERAL
	:  '"' ( ~'"' | '""' )* '"'
	;

HEX_LITERAL
	: '&' ('H'|'h') HexDigit+ { Text = Text.Substring(2); }
	;

fragment
DECIMAL_LITERAL
	: // ('0' | '1'..'9' '0'..'9'*) NumericTypeSuffix?
	;

fragment
HexDigit : ('0'..'9'|'a'..'f'|'A'..'F') ;

FLOATING_POINT_LITERAL
	:
		('0'..'9') Digits?
		( { char.IsDigit((char)input.LA(2)) }? => '.' Digits? Exponent? FloatTypeSuffix? { $type = FLOATING_POINT_LITERAL; }
		| Exponent FloatTypeSuffix? { $type = FLOATING_POINT_LITERAL; }
		| NumericTypeSuffix? { $type = DECIMAL_LITERAL; }
		)
	|
		'.'
		( Digits Exponent? FloatTypeSuffix? { $type = FLOATING_POINT_LITERAL; }
		| { $type = DOT; }
		)
	;


fragment
Digits
	:   ('0'..'9')+
	;

fragment
Exponent
	:   ('e'|'E') ('+'|'-')?
		( Digits
		|
			{
				EmitErrorMessage("Malformed exponent");
				Text = "0.0";
			}
		)
	;

fragment
NumericTypeSuffix
	: ('U'|'u')('L'|'l')
	| ('L'|'l')
	| ('U'|'u')('S'|'s')
	| ('S'|'s')
	| ('U'|'u')('I'|'i')
	| ('I'|'i')
	| ('C'|'c')
	| FloatTypeSuffix
	;

fragment
FloatTypeSuffix
	: ('F'|'f'|'R'|'r'|'D'|'d')
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
