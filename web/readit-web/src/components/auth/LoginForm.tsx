import { useForm } from 'react-hook-form'
import { useAuthStore } from '../../stores/authStore'
import { useNavigate } from 'react-router-dom'
import { useState } from 'react'
import { RequestLoginJson } from '../../types/auth';
import { ClipLoader } from 'react-spinners';

export default function LoginForm() {
    const { register, handleSubmit, formState: { errors } } = useForm<RequestLoginJson>()
    const { login } = useAuthStore()
    const navigate = useNavigate()
    const [submitError, setSubmitError] = useState('')
    const [loading, setLoading] = useState(false)

    const onSubmit = async (data: RequestLoginJson) => {
      setLoading(true)
      setSubmitError('')

        try {
            const response = await login(data);
            localStorage.setItem('token', response.accessToken);
            navigate('/');
        } catch {
            setSubmitError('E-mail ou senha inválidos');
        } finally {
          setLoading(false)
        }
    }

    return (
        <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
          {submitError && (
            <div className="bg-red-100 border border-red-400 text-red-700 px-4 py-3 rounded">
              {submitError}
            </div>
          )}
    
          <div>
            <input placeholder='E-mail'
              type="email"
              disabled={loading}
              {...register('email', { 
                required: 'Campo obrigatório',
                pattern: { value: /^\S+@\S+$/i, message: 'E-mail inválido' }
              })}
              className="w-full h-16 p-4 border rounded-lg text-lg"
            />
            {errors.email && <span className="text-red-500 text-sm">{errors.email.message}</span>}
          </div>
    
          <div>
            <input placeholder='Senha'
              type="password"
              disabled={loading}
              {...register('password', { 
                required: 'Campo obrigatório'
              })}
              className="w-full h-16 p-4 border rounded-lg text-lg"
            />
            {errors.password && <span className="text-red-500 text-sm">{errors.password.message}</span>}
          </div>
    
          <button 
            type="submit"
            disabled={loading}
            className="w-full h-14 bg-readit-light text-readit-primary font-bold p-2 rounded-lg hover:bg-slate-100 transition-colors"
          >
            {loading ? (
              <ClipLoader size={20} color="#14213d" />
            ) : (
              'Entrar'
            )}
          </button>
        </form>
      )
}