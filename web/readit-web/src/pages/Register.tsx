import { Link } from 'react-router-dom'
import RegisterForm from '../components/auth/RegisterForm'
import { ArrowLeft } from 'lucide-react';

export default function Register() {
  return (
    <div className="min-h-screen flex flex-col md:flex-row items-center justify-center bg-readit-primary p-4">
      <div className="md:mr-16 w-full max-w-md order-1 md:order-none">
        <h1 className="irish-grover text-[64px] text-readit-light text-center md:text-left">READIT</h1>
        <h2 className="text-[36px] mb-6 text-readit-light mt-[60px]">Cadastro</h2>
        <p className="text-lg mb-6 text-readit-light">
          Faça seu cadastro, entre na plataforma e leia seus mangas favoritos
        </p>
        
        {/* Link "Voltar" - Visível APENAS em desktop */}
        <div className="hidden md:block mt-6">
          <Link 
            to="/login" 
            className="flex items-center gap-1 text-lg text-readit-light font-bold"
          >
            <ArrowLeft className="w-5 h-5" />
            Voltar para o login
          </Link>
        </div>
      </div>

      <div className="w-full max-w-lg order-2 md:order-none">
        <RegisterForm />
        
        {/* Link "Voltar" - Visível APENAS em mobile */}
        <div className="mt-6 md:hidden text-center">
          <Link 
            to="/login" 
            className="flex items-center justify-center gap-1 text-lg text-readit-light font-bold"
          >
            <ArrowLeft className="w-5 h-5" />
            Voltar para o login
          </Link>
        </div>
      </div>
    </div>
  )
}