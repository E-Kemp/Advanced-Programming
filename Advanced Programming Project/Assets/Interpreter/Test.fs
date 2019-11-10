namespace Test
open System.Text.RegularExpressions


(*
Hook type to handle entity interaction
*)
type Hook(hook: string) =
    let event = new Event<unit>()
    let mutable trigBool = false
    
    member this.Trigger =
        if trigBool then trigBool <- false
        else trigBool <- true
        event.Trigger()

    member this.IsTriggered = trigBool
    member this.Hook = hook
    member this.EventHandler = event.Publish


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
    | INT of int
    | BOOLEAN of bool
    | LAMBDA of Expr
and Expr =
    | TRIGGER of string
    | IF of Expr * Expr * Expr

type Token =
    | OP of Operator
    | VA of Value
    | NULL

module Test =

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
            | VA(HOOK(x)) -> sprintf "(HOOK: %s)" x
            | _ -> ""


    let rec printTokensRec(str, tokens:List<Token>) =
        if tokens.IsEmpty = false then
            let tmp = printToken(tokens.Head)
            printTokensRec(tmp+str, tokens.[1..])
        else str

    let printTokens(str:string) = 
        let strList = str.Split [|' '|] |> Array.toList
        let tokens = tokenise(strList, List.empty<Token>)
        printTokensRec("", tokens)