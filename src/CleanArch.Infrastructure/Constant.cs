namespace CleanArch.Infrastructure;

public static class Constant
{
    public static readonly string Empty = "";
    public static readonly string Space = " ";
    public static readonly char CharComma = ',';
    public static readonly char CharSpace = ' ';
    public static readonly string StringComma = ",";
    public static readonly string JwtKey = "JwtSettings:JwtKey";
    public static readonly string Issuer = "JwtSettings:JwtIssuer";
    public static readonly string Audience = "JwtSettings:JwtAudience";
    public static readonly string Migrations_Folder = "CleanArch.Infrastructure";
    public static readonly string AppDbConnection = "AppDbConnection";
    public static readonly string AppDbConnection_Error = $"Connection string '{AppDbConnection}' not found.";
    public static readonly string LogDbConnection = "LogDbConnection";
    public static readonly string LogDbConnection_Error = $"Connection string '{LogDbConnection}' not found.";
}
