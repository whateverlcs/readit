import { Link } from 'react-router-dom'
import { LogOut } from 'lucide-react';
import LoginForm from '../components/auth/LoginForm';
import LoginImage from '../../../../core/Readit.Resources/Images/login-image.png'

export default function Login() {
  return (
    <div className="min-h-screen flex flex-col md:flex-row items-center justify-center bg-readit-primary p-4 md:p-8">
      <div className="w-full md:w-auto md:min-w-[400px] md:mr-8 lg:mr-16 text-center md:text-left">
        <h1 className="irish-grover text-[64px] text-readit-light mb-4">READIT</h1>
        <h2 className="text-[28px] md:text-[36px] mb-6 text-readit-light">Faça seu logon</h2>
        <LoginForm />
        <div className="mt-6 text-lg text-readit-light font-bold">
          <Link 
            to="/register" 
            className="flex items-center justify-center md:justify-start gap-2"
          >
            <LogOut className="w-5 h-5 text-red-600" />
            Não tenho cadastro
          </Link>
        </div>
      </div>

      <div className="hidden md:flex flex-1 max-w-[600px] items-center justify-center p-4 mb-20">
        <img 
          src={LoginImage} 
          className="w-full h-auto max-h-[70vh] object-contain transition-all duration-300"
          alt="Ilustração de login"
        />
      </div>
    </div>
  )
}