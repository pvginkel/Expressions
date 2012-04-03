grammar Csharp;

options
{
	language = CSharp3;
	TokenLabelType = CommonToken;
	output = AST;
	ASTLabelType = CommonTree;
	backtrack=true;
	memoize=true;
	k=2;
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

@lexer::namespace { Expressions.Csharp }
@parser::namespace { Expressions.Csharp }
@lexer::modifier { internal }
@parser::modifier { internal }

prog returns [IAstNode value]
	: expression { $value = $expression.value; } EOF
	;

expression returns [IAstNode value]
	: e=conditional_expression { $value = $e.value; }
	;

conditional_expression returns [IAstNode value]
	: e=logical_or_expression { $value = $e.value; }
		( '?' t=expression ':' l=expression { $value = new Conditional($e.value, $t.value, $l.value); }
		)?
	;

logical_or_expression returns [IAstNode value]
	: e=logical_and_expression { $value = $e.value; }
		( '||' e=logical_and_expression
		  { $value = new BinaryExpression($value, $e.value, ExpressionType.LogicalOr); }
		)*
	;

logical_and_expression returns [IAstNode value]
	: e=bitwise_or_expression { $value = $e.value; }
		( '&&' e=bitwise_or_expression
		  { $value = new BinaryExpression($value, $e.value, ExpressionType.LogicalAnd); }
		)*
	;
	
bitwise_or_expression returns [IAstNode value]
	: e=bitwise_xor_expression { $value = $e.value; }
		( '|' e=bitwise_xor_expression
		  { $value = new BinaryExpression($value, $e.value, ExpressionType.BitwiseOr); }
		)*
	;
	
bitwise_xor_expression returns [IAstNode value]
	: e=bitwise_and_expression { $value = $e.value; }
		( '^' e=bitwise_and_expression
		  { $value = new BinaryExpression($value, $e.value, ExpressionType.Xor); }
		)*
	;
		
bitwise_and_expression returns [IAstNode value]
	: e=unary_not_expression { $value = $e.value; }
		( '&' e=unary_not_expression
		  { $value = new BinaryExpression($value, $e.value, ExpressionType.BitwiseAnd); }
		)*
	;

unary_not_expression returns [IAstNode value]
	: e=equality_expression { $value = $e.value; }
	| '!' u=unary_not_expression { $value = new UnaryExpression($u.value, ExpressionType.LogicalNot); }
	| '~' u=unary_not_expression { $value = new UnaryExpression($u.value, ExpressionType.BitwiseNot); }
	;

equality_expression returns [IAstNode value]
	: e=relational_expression { $value = $e.value; }
		(
			( '==' e=relational_expression { $value = new BinaryExpression($value, $e.value, ExpressionType.Equals); }
			| '!=' e=relational_expression { $value = new BinaryExpression($value, $e.value, ExpressionType.NotEquals); }
			)
		)*
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
		( '*' e=cast_expression { $value = new BinaryExpression($value, $e.value, ExpressionType.Multiply); }
		| '/' e=cast_expression { $value = new BinaryExpression($value, $e.value, ExpressionType.Divide); }
		| '%' e=cast_expression { $value = new BinaryExpression($value, $e.value, ExpressionType.Modulo); }
		)*
	;

cast_expression returns [IAstNode value]
	: '(' t=type_expression ')' e=cast_expression { $value = new Cast($e.value, $t.value); }
	| u=unary_expression { $value = $u.value; }
	;

type_expression returns [TypeIdentifier value]
	: e=type_identifier { $value = new TypeIdentifier($e.value, 0); }
		(
			'[' { $value = new TypeIdentifier($value.Name, 1); }
				( ',' { $value = new TypeIdentifier($value.Name, $value.ArrayIndex + 1); }
				)*
			']'
		)?
	;

type_identifier returns [string value]
	: e=IDENTIFIER { $value = $e.text; } ( DOT e=IDENTIFIER { $value = $value + "." + $e.text; } )*
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
		( '[' e=argument_expression_list ']' { $value = new Index($value, $e.value); }
		| '(' ')' { $value = new MethodCall($value); }
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
	: 'true' { $value = Constant.True; }
	| 'false' { $value = Constant.False; }
	| 'null' { $value = Constant.Null; }
	| DATETIME_LITERAL { $value = ParseDateTime($DATETIME_LITERAL.text); }
	| TIMESPAN_LITERAL { $value = ParseTimeSpan($TIMESPAN_LITERAL.text); }
	| HEX_LITERAL { $value = ParseHex($HEX_LITERAL.text); }
	| DECIMAL_LITERAL { $value = ParseDecimal($DECIMAL_LITERAL.text); }
	| CHARACTER_LITERAL { $value = ParseCharacter($CHARACTER_LITERAL.text); }
	| STRING_LITERAL { $value = ParseString($STRING_LITERAL.text); }
	| FLOATING_POINT_LITERAL { $value = ParseFloatingPoint($FLOATING_POINT_LITERAL.text); }
	;

DOT
	: // '.'
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

fragment
HEX_LITERAL
	: // '0' ('x'|'X') HexDigit+ NumericTypeSuffix?
	;

fragment
DECIMAL_LITERAL
	: // ('0' | '1'..'9' '0'..'9'*) NumericTypeSuffix?
	;

fragment
HexDigit : ('0'..'9'|'a'..'f'|'A'..'F') ;

FLOATING_POINT_LITERAL
	:
		'0' +
		(
			('x'|'X') { $type = HEX_LITERAL; }
			(
				('0'..'9'|'a'..'z'|'A'..'Z')+
				{ Text = Text.Substring(2); }
			)
			| { char.IsDigit((char)input.LA(2)) }? => '.' Digits Exponent? FloatTypeSuffix? { $type = FLOATING_POINT_LITERAL; }
			| NumericTypeSuffix? { $type = DECIMAL_LITERAL; }
		)
	|	('1'..'9') Digits?
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
	: ('u' | 'U') ('l' | 'L')?
	| ('l' | 'L') ('u' | 'U')?
	| FloatTypeSuffix
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
