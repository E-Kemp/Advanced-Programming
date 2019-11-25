namespace Interpreter
open Unity
open System
open System.Text.RegularExpressions

//type Ent = LevelController.LevelEntity //Abbreviating entities

type Operator =
    | ADD
    | SUBTRACT

type Value =
    | HOOK of string
    | START
    | END
and Expr =
    | AND
type Token =
    | OP of Operator
    | VA of Value
    | EX of Expr
    | NULL

type Tree =
    | Node1 of Node
    | Leaf of Token
and Node = { token:Token; child:Tree[] }

(*
Lexer module to tokenise a given string of code form the game
*)
module Lexer =            
    (* Get the keyword out of the next valid chain of alphanumeric characters *)
    let getKeyword str =
        let reg = Regex.Match(str, "[A-z]+")
        match reg.Success with
        | true -> reg.Groups.[0].Value
        | false -> ""

    let rec tokenise(str:string, tokenList) =
        if str.Length = 0 then
            List.rev tokenList
        else match str.[0] with
                | '+' ->        tokenise(str.[1..], OP(ADD) :: tokenList)
                | '-' ->        tokenise(str.[1..], OP(SUBTRACT) :: tokenList)
                | '(' ->        tokenise(str.[1..], VA(START) :: tokenList)
                | ')' ->        tokenise(str.[1..], VA(END) :: tokenList)
                | ' ' ->        tokenise(str.[1..], tokenList)
                (* Go into keywords after characters *)
                | _ ->          let s = getKeyword(str)
                                match s with
                                | "AND" ->   tokenise(str.[3..], EX(AND) :: tokenList)
                                (* Nothing else -> grab the token *)
                                | _ ->      if String.length(s) > 0 then
                                                tokenise(str.[s.Length..], VA(HOOK(s)) :: tokenList)
                                            else
                                                tokenise(str.[1..], tokenList)
                                


    let printToken token =
        match token with
            | OP(ADD) -> "(ADD)"
            | OP(SUBTRACT) -> "(SUBTRACT)"
            //| OP(_) -> "(BLANK OPERATION)"
            | VA(START) -> "(START)"
            | VA(END) -> "(END)"            
            | VA(HOOK(a)) -> sprintf "(HOOK: %s)" a
            | EX(AND) -> "(AND)"
            | _ -> "(NULL)"
    

    let getTokens str =
        tokenise(str, List.empty<Token>)

    let rec printTokensRec(str, tokens:List<Token>) =
        if tokens.IsEmpty = false then
            let tmp = printToken(tokens.Head)
            printTokensRec(tmp+str, tokens.[1..])
        else str
    
    let printTokens tokens =
        printTokensRec("", List.rev tokens)
    
    let printTokensStr str = 
        let tokens = getTokens str
        printTokens tokens



(*
Parser module to parse and execute the tokens given by the lexer
*)
module Parser =
    let test = 0

    let separateHooks tokens = 
        tokens |> List.choose ( fun a -> match a with | VA(HOOK(b)) -> Some b | _ -> None )

    let rec validateHooks(ents:List<(string * bool)>, hooks:List<string>) =
        if List.exists (fun (a, _) -> a = hooks.[0]) ents then
            if hooks.Length <= 1 then true else
            validateHooks(ents, hooks.[1..])
        else
            false
    
    let rec buildTree(tokens:List<Token>, pos:int, currentTree:Tree) =
        match tokens.[pos] with
        | EX(AND) -> ""
        | _ -> ""
(*
Interpreter module to trigger the process of interpreting code
*)
module Main =
    open Lexer
    open Parser

    let printEnts ents =
        ents |> List.map<(string * bool), string> (fun (a, b) -> a + ", " + match b with | true -> "true" | false -> "false")    
    


    let interpret(str, hooks, triggers) =
        let ents = Seq.zip hooks triggers |> List.ofSeq // First create a sequence of entities, this is cool
        let tokens = getTokens str
        if validateHooks(ents, separateHooks tokens) then
            buildTree(tokens, 0, Leaf(NULL))
        else
            ""
        
        
        
        
        //(Lexer.printTokens tokens) 
        //+ "\n" 
        //+ (ents 
        //    |> List.map<(string * bool), string> (fun (a,_) -> a)
        //    |> List.fold (+) " ")
