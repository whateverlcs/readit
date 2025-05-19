import { create } from 'zustand';
import api from '../api/client';
import { RegisterFormData, RegisterResponse, RequestLoginJson, ResponseLoginJson } from '../types/auth';

interface AuthState {
  register: (data: RegisterFormData) => Promise<RegisterResponse>;
  login: (data: RequestLoginJson) => Promise<ResponseLoginJson>;
}

export const useAuthStore = create<AuthState>(() => ({
  register: async (data) => {
    try {
      await api.post('/Usuario', data);
      return { success: true };
    } catch (error: unknown) {
      // Tratamento de erro type-safe
      if (typeof error === 'object' && error !== null && 'response' in error) {
        const apiError = error as {
          response?: {
            data?: {
              messages?: string[];
            };
          };
        };
        
        return {
          success: false,
          error: apiError.response?.data?.messages?.join(', ') || 'Erro no cadastro'
        };
      }
      
      return {
        success: false,
        error: 'Erro desconhecido'
      };
    }
  },
  login: async (data) => {
    const response = await api.post('/Login', data);
    return response.data;
  },
}));