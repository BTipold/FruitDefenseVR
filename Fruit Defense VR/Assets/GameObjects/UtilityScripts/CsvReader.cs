// ---------------------------------------------------------------
// Copyright (C) 2019-2020 TwinTip Studios - All Rights Reserved
// This software and all related information is the intellectual
// property of TwinTip Studios and may not be distributed,
// replicated or disclosed without the explicit prior written
// permission of TwinTip Studios. 
// ---------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CsvReader
{
    private string mRawCsv;
    private List<List<string>> mData2D = new List<List<string>>();

    public CsvReader()
    {
    }

    public CsvReader( TextAsset rawCsv )
    {
        Open( rawCsv );
    }

    public void Open( TextAsset csv )
    {
        // Check if text file is valid
        if ( csv != null )
        {
            mRawCsv = csv.text;
            Parse();
        }
        else
        {
            Debug.LogError( "CsvReader cannot open because CSV=null" );
        }
    }

    public void Parse()
    {
        // Remove CRLF line endings and split by \n
        string[] rows = mRawCsv.Replace("\r","").Replace("\\,","$COMMA$").Split('\n');

        // Split by commas to build array of cells
        foreach ( string row in rows)
        {
            if ( row != "" )
            {
                string[] splitRow = row.Split( ',' );
                foreach(string s in splitRow)
                {
                    s.Replace( "$COMMA$", "," ); 
                }
                mData2D.Add(new List<string>( splitRow ));
            }
        }
    }

    public string GetCell(int rowIndex, int columnIndex)
    {
        string cell = "";

        if ( mData2D.Count >= rowIndex )
        {
            List<string> row = mData2D[rowIndex];
            if ( row.Count >= columnIndex )
            {
                cell = row[columnIndex];
            }
            else
            {
                Debug.LogError( "Column " + columnIndex + 
                    " out of bounds (size=" + row.Count + ")" );
            }
        }
        else
        {
            Debug.LogError( "Row " + rowIndex +
                " out of bounds (size=" + mData2D.Count + ")" );
        }

        return cell;
    }

    public string GetDataUnderHeading(string key, int rowIndex = 1)
    {
        string value = "";

        foreach ( List<string> row in mData2D )
        {
            if(row.Contains(key))
            {
                int columnIndex = row.IndexOf( key );
                value = GetCell(rowIndex, columnIndex);
                break;
            }
            rowIndex++;
        }

        return value;
    }

    public List<string> GetColumn( int index )
    {
        List<string> column = new List<string>();

        foreach( List<string> row in mData2D )
        {
            if( row.Count >= index )
            {
                column.Add( row[index] );
            }
        }

        return column;
    }

    public int NumRows()
    {
        return mData2D.Count;
    }
}
