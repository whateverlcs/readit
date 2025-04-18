﻿namespace Readit.Core.Domain
{
    public class Usuario
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Apelido { get; set; }
        public string Email { get; set; }
        public string Senha { get; set; }
        public bool Administrador { get; set; }
        public int? IdImagem { get; set; }
        public byte[] ImageByte { get; set; }
    }
}