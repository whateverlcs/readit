import { useForm } from 'react-hook-form'
import { useAuthStore } from '../../stores/authStore'
import { useNavigate } from 'react-router-dom'
import { useState, useEffect } from 'react'
import { RegisterFormData } from '../../types/auth';
import { ClipLoader } from 'react-spinners';

export default function RegisterForm() {
  const { register, handleSubmit, formState: { errors }, reset } = useForm<RegisterFormData>()
  const { register: registerUser } = useAuthStore()
  const navigate = useNavigate()
  const [submitError, setSubmitError] = useState('')
  const [loading, setLoading] = useState(false)
  const [successMessage, setSuccessMessage] = useState('')

  useEffect(() => {
    if (successMessage) {
      const timer = setTimeout(() => {
        setSuccessMessage('')
      }, 5000)
      return () => clearTimeout(timer)
    }
  }, [successMessage])

  const onSubmit = async (data: RegisterFormData) => {
    setLoading(true)
    setSubmitError('')
    setSuccessMessage('')

    try{
      const { success, error } = await registerUser(data)
      if (success) {
        setSuccessMessage('Cadastro realizado com sucesso!')
        reset()
      } else {
        setSubmitError(error || 'Erro desconhecido')
      }
    } catch{
      setSubmitError('Erro ao realizar o cadastro');
    } finally{
      setLoading(false)
    }
  }

  return (
    <div className="relative">
      {successMessage && (
        <div className="fixed top-4 right-4 z-50">
          <div className="bg-green-500 text-white px-6 py-3 rounded-md shadow-lg">
            {successMessage}
          </div>
        </div>
      )}

      <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
        {submitError && (
          <div className="bg-red-100 border border-red-400 text-red-700 px-4 py-3 rounded">
            {submitError}
          </div>
        )}

        <div>
          <input placeholder='Nome Completo'
            disabled={loading}
            {...register('nome', { required: 'Campo obrigatório' })}
            className="w-full h-16 p-4 border rounded-lg text-lg"
          />
          {errors.nome && <span className="text-red-500 text-sm">{errors.nome.message}</span>}
        </div>

        <div>
          <input placeholder='Apelido'
            disabled={loading}
            {...register('apelido', { required: 'Campo obrigatório' })}
            className="w-full h-16 p-4 border rounded-lg text-lg"
          />
          {errors.apelido && <span className="text-red-500 text-sm">{errors.apelido.message}</span>}
        </div>

        <div>
          <input placeholder='E-mail'
            type="email"
            disabled={loading}
            {...register('email', { 
              required: 'Campo obrigatório',
              pattern: { value: /^\S+@\S+$/i, message: 'Email inválido' }
            })}
            className="w-full h-16 p-4 border rounded-lg text-lg"
          />
          {errors.email && <span className="text-red-500 text-sm">{errors.email.message}</span>}
        </div>

        <div>
          <input placeholder='Senha'
            type="password"
            disabled={loading}
            {...register('senha', { 
              required: 'Campo obrigatório',
              minLength: { value: 6, message: 'Mínimo 6 caracteres' }
            })}
            className="w-full h-16 p-4 border rounded-lg text-lg"
          />
          {errors.senha && <span className="text-red-500 text-sm">{errors.senha.message}</span>}
        </div>

        <button 
          type="submit" 
          disabled={loading}
          className="w-full h-14 bg-readit-light text-readit-primary font-bold p-2 rounded-lg hover:bg-slate-100 transition-colors"
        >
          {loading ? (
                <ClipLoader size={20} color="#14213d" />
              ) : (
                'Cadastrar'
              )}
        </button>
      </form>
    </div>
  )
}