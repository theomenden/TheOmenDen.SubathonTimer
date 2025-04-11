namespace TheOmenDen.SubathonTimer.Models.Enums;

/// <summary>
/// Defines the transaction modes for IndexedDB.
/// </summary>
public enum TransactionMode
{
    // Base Modes
    /// <summary>Read-only mode.</summary>
    /// <value>'r'</value>
    Read,
    /// <summary>Write-only mode.</summary>
    /// <value>'w'</value>
    Write,
    /// <summary>Read-write mode.</summary>
    /// <value>'rw'</value>
    ReadWrite,

    // Parent Reused Modes (only when compatible)
    /// <summary>Read-only mode with parent reuse.</summary>
    /// <value>
    /// 'r?'
    /// </value>
    ReadQ,
    /// <summary>Write-only mode with parent reuse.</summary>
    /// <value>
    /// 'w?'
    /// </value>
    WriteQ,
    /// <summary>Read-write mode with parent reuse.</summary>
    /// <value>
    /// 'rw?'
    /// </value>
    ReadWriteQ,

    // Force Top-Level Modes
    /// <value>
    /// 'r!'
    /// </value>
    ReadEx,
    /// <value>
    /// 'w!'
    /// </value>
    WriteEx,
    /// <value>
    /// 'rw!'
    /// </value>
    ReadWriteEx
}