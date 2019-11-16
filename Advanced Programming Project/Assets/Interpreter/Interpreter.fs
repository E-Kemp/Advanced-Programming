namespace Interpreter
open System
open System.Text.RegularExpressions
open UnityEngine


type Operator =
    | ADD
    | SUBTRACT

type Value =
    | HOOK of string
    | START
    | END
and Expr =
    | IF
    | FOR

type Token =
    | OP of Operator
    | VA of Value
    | EX of Expr
    | NULL

(*
Lexer module to tokenise a given string of code form the game
*)
module Lexer =
    //let (|Regex|_|) pattern input =
    //    let m = Regex.Match(input, pattern)
    //    if m.Success then Some(List.tail [ for g in m.Groups -> g.Value ])
    //    else None
    
    
    let rec getVar(str:string, cstr:string) =
        if str.Length = 0 then
            (cstr.Length, cstr)
        else    
            match str.[0] with
            | ')' -> (cstr.Length, cstr)
            | ' ' -> (cstr.Length, cstr)
            | _ -> getVar(str.[1..], (cstr + string str.[0])) // WORK ON THIS

            


    let rec tokenise(str:string, tokenList:List<Token>) =
        if str.Length = 0 then
            tokenList

        else match str.[0] with
                | '+' ->        tokenise(str.[1..], OP(ADD) :: tokenList)
                | '-' ->        tokenise(str.[1..], OP(SUBTRACT) :: tokenList)
                | '(' ->        tokenise(str.[1..], VA(START) :: tokenList)
                | ')' ->        tokenise(str.[1..], VA(END) :: tokenList)
                | 'H' ->        if str.[0..3] = "HOOK" && str.Length >= 5 then 
                                    let (a, b) = getVar(str.[5..], "")
                                    Console.WriteLine a
                                    Console.WriteLine b
                                    Console.WriteLine str.[a..]
                                    tokenise(str.[a..], VA(HOOK(b)) :: tokenList)
                                else
                                    List.rev tokenList
                | ' ' ->        tokenise(str.[1..], tokenList)
                | _ ->          List.rev tokenList

        


    let printToken(token:Token) =
        match token with
            | OP(ADD) -> "(ADD)"
            | OP(SUBTRACT) -> "(SUBTRACT)"
            //| OP(_) -> "(BLANK OPERATION)"
            | VA(HOOK(a)) -> sprintf "(HOOK: %s)" a
            | VA(START) -> "(START)"
            | VA(END) -> "(END)"
            | _ -> "(NULL)"
    

    let rec printTokensRec(str, tokens:List<Token>) =
        if tokens.IsEmpty = false then
            let tmp = printToken(tokens.Head)
            printTokensRec(tmp+str, tokens.[1..])
        else str

    let getTokens(str:string) =
        tokenise(str, List.empty<Token>)

    let printTokensStr(str:string) = 
        let tokens = getTokens str
        printTokensRec("", tokens)

    let printTokens(tokens:List<Token>) =
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
        if List.exists (fun (a, _) -> a = str) HOOKS then
            HOOKS <- List.append HOOKS [(str, trigger)]
        else
            Console.Write("hook already exists!")

    let rec getHook str =
        (HOOKS |> List.filter (fst >> (=) str)).[0]
    //let getHook str = 
    //    List.

    //let interpret str:string =
    //    let strArr = str.
    //    let tokenList = tokenise(str.Split(' '), List.empty<Token>)
    //    ""