export interface RegisterFormData {
  nome: string;
  apelido: string;
  email: string;
  senha: string;
}

export interface RegisterResponse {
  success: boolean;
  error?: string;
}

export interface ApiError {
  messages: string[]
}

export interface RequestLoginJson {
  email: string;
  password: string;
}

export interface ResponseLoginJson {
  nome: string;
  accessToken: string;
}