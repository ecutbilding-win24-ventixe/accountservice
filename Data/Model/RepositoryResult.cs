﻿namespace Data.Model;

public class RepositoryResult
{
    public bool Succeeded { get; set; }
    public int StatusCode { get; set; }
    public string? Message { get; set; }
}

public class RepositoryResult<T> : RepositoryResult
{
    public T? Result { get; set; }
}
