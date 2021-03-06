﻿namespace WebApiKata.Interfaces
{
    public interface ISerializer
    {
        string Serialize(object obj);

        T Deserialize<T>(string serializedObject);
    }
}
