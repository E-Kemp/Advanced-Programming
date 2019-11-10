namespace Interpreter
open System
open System.Text.RegularExpressions
open UnityEngine


type Operator =
    | ADD
    | SUBTRACT
    | MULTIPLY
    | DIVIDE
    | MODULUS
    | EXPONENT
    | WHITESPACE
    | TEST

type Value =
    | HOOK of string
    | LAMBDA of Expr
and Expr =
    | TOGGLE of string
    | IF of (Expr * Expr * Expr)

type Token =
    | OP of Operator
    | VA of Value
    | NULL

(*
Lexer module to tokenise a given string of code form the game
*)
module Lexer =
    let (|Regex|_|) pattern input =
        let m = Regex.Match(input, pattern)
        if m.Success then Some(List.tail [ for g in m.Groups -> g.Value ])
        else None

    let rec tokenise(str:List<string>, tokenList:List<Token>) =
        if str.Length = 0 then
            tokenList
        else match str.[0] with
                | "+" ->                tokenise(str.[1..], OP(ADD) :: tokenList)
                | "-" ->                tokenise(str.[1..], OP(SUBTRACT) :: tokenList)
                | "*" ->                tokenise(str.[1..], OP(MULTIPLY) :: tokenList)
                | "/" ->                tokenise(str.[1..], OP(DIVIDE) :: tokenList)
                | "%" ->                tokenise(str.[1..], OP(MODULUS) :: tokenList)
                | "^" ->                tokenise(str.[1..], OP(EXPONENT) :: tokenList)
                | "HOOK" ->             if str.Length > 1 then
                                            tokenise(str.[2..], VA(HOOK(str.[1])) :: tokenList)
                                        else
                                            tokenList
                //| "LAMBDA" ->           if str.Length > 3 then // this is broke fucking fix it
                | _ -> tokenList

    let printToken(token:Token) =
        match token with
            | OP(ADD) -> "(ADD)"
            | OP(SUBTRACT) -> "(SUBTRACT)"
            | OP(MULTIPLY) -> "(MULTIPLY)"
            | OP(DIVIDE) -> "(DIVIDE)"
            | OP(MODULUS) -> "(MODULUS)"
            | OP(EXPONENT) -> "(EXPONENT)"
            | OP(_) -> "(BLANK OPERATION)"
            | VA(HOOK(a)) -> sprintf "(HOOK: %s)" a
            | _ -> ""
    

    let rec printTokensRec(str, tokens:List<Token>) =
        if tokens.IsEmpty = false then
            let tmp = printToken(tokens.Head)
            printTokensRec(tmp+str, tokens.[1..])
        else str

    let getTokens(str:string) =
        let strList = str.Split [|' '|] |> Array.toList
        tokenise(strList, List.empty<Token>)

    let printTokens(str:string) = 
        let tokens = getTokens str
        printTokensRec("", tokens)






(*
Parser module to parse and execute the tokens given by the lexer
*)
module Parser =
    let test = 0




(*
Interpreter module to trigger the process of interpreting code
*)
module Interpreter =
    open Lexer
    open Parser

    // Mutable list of hooks
    // Yes, I know mutables aren't pretty but this needs to be changeable in game
    let mutable HOOKS = List.empty<(string * bool)>

    //Add hook to the list of hooks
    let addHook str trigger =
        HOOKS <- List.append HOOKS [(str, trigger)]

    let rec getHook str =
        (HOOKS |> List.filter (fst >> (=) str)).[0]
    //let getHook str = 
    //    List.

    //let interpret str:string =
    //    let strArr = str.
    //    let tokenList = tokenise(str.Split(' '), List.empty<Token>)
    //    ""