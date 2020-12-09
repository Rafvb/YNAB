namespace Raf.YNAB.Importer

open System.Globalization
open FSharp.Data

type Transactions = CsvProvider<"sample.csv", EmbeddedResource="Raf.YNAB.Importer, Raf.YNAB.Importer.sample.csv", Separators=";">
type YNABTransactions = CsvProvider<"ynab.csv", Schema="Date (string), Payee (string), Memo (string), Outflow (string), Inflow (string)",
                                                EmbeddedResource="Raf.YNAB.Importer, Raf.YNAB.Importer.ynab.csv">

module FortisParser =
    let parse file =
        let transactions = Transactions.Parse(file)

        let ynabTransactions =  
            transactions.Rows             
            |> Seq.map (fun row -> 
                let formattedBedrag = (abs row.Bedrag).ToString("F", CultureInfo.InvariantCulture)

                let outflow = if row.Bedrag < 0m then formattedBedrag else ""
                let inflow = if row.Bedrag > 0m then formattedBedrag else ""

                YNABTransactions.Row(row.Uitvoeringsdatum, row.Details, row.Details, outflow, inflow))
            |> Seq.toList

        let ynab = new YNABTransactions(ynabTransactions)
        ynab.SaveToString()
