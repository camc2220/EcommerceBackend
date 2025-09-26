import React, { useEffect, useState } from 'react'

const API_BASE = import.meta.env.VITE_API_BASE_URL || 'http://localhost:5000/api'

const initialFormState = {
  fullName: '',
  email: '',
  password: ''
}

export default function App() {
  const [products, setProducts] = useState([])
  const [registerForm, setRegisterForm] = useState(initialFormState)
  const [registerMessage, setRegisterMessage] = useState('')
  const [registerError, setRegisterError] = useState('')
  const [isSubmitting, setIsSubmitting] = useState(false)

  useEffect(() => {
    fetch(`${API_BASE}/products`)
      .then(r => r.json())
      .then(setProducts)
      .catch(() => {})
  }, [])

  const handleInputChange = event => {
    const { name, value } = event.target
    setRegisterForm(prev => ({ ...prev, [name]: value }))
  }

  const handleRegister = async event => {
    event.preventDefault()
    setRegisterMessage('')
    setRegisterError('')
    setIsSubmitting(true)

    try {
      const response = await fetch(`${API_BASE}/auth/register`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({
          fullName: registerForm.fullName || null,
          email: registerForm.email,
          password: registerForm.password
        })
      })

      const data = await response.json().catch(() => ({}))

      if (!response.ok) {
        setRegisterError(data.error || data.message || 'No se pudo completar el registro.')
        return
      }

      setRegisterMessage(data.message || 'Registro completado.')
      setRegisterForm(initialFormState)
    } catch (error) {
      setRegisterError('No se pudo conectar con el servidor.')
    } finally {
      setIsSubmitting(false)
    }
  }

  return (
    <div style={{ padding: 20, fontFamily: 'Arial', maxWidth: 960, margin: '0 auto' }}>
      <h1>E-commerce Demo</h1>

      <section style={{ marginBottom: 32, padding: 20, border: '1px solid #ddd', borderRadius: 12 }}>
        <h2>Crear cuenta</h2>
        <form onSubmit={handleRegister} style={{ display: 'grid', gap: 12, maxWidth: 400 }}>
          <label style={{ display: 'grid', gap: 4 }}>
            Nombre completo (opcional)
            <input
              type="text"
              name="fullName"
              value={registerForm.fullName}
              onChange={handleInputChange}
              placeholder="Ana Pérez"
              style={{ padding: 8, borderRadius: 6, border: '1px solid #ccc' }}
            />
          </label>

          <label style={{ display: 'grid', gap: 4 }}>
            Correo electrónico
            <input
              type="email"
              name="email"
              value={registerForm.email}
              onChange={handleInputChange}
              placeholder="ana@example.com"
              required
              style={{ padding: 8, borderRadius: 6, border: '1px solid #ccc' }}
            />
          </label>

          <label style={{ display: 'grid', gap: 4 }}>
            Contraseña
            <input
              type="password"
              name="password"
              value={registerForm.password}
              onChange={handleInputChange}
              placeholder="Mínimo 6 caracteres"
              required
              minLength={6}
              style={{ padding: 8, borderRadius: 6, border: '1px solid #ccc' }}
            />
          </label>

          <button
            type="submit"
            disabled={isSubmitting}
            style={{
              padding: '10px 16px',
              borderRadius: 6,
              border: 'none',
              background: '#0d6efd',
              color: 'white',
              cursor: 'pointer'
            }}
          >
            {isSubmitting ? 'Creando cuenta…' : 'Registrarme'}
          </button>
        </form>

        {registerError && (
          <p style={{ color: '#b91c1c', marginTop: 16 }}>{registerError}</p>
        )}

        {registerMessage && (
          <p style={{ color: '#047857', marginTop: 16 }}>{registerMessage}</p>
        )}
      </section>

      <section>
        <h2>Productos destacados</h2>
        <div style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fill, minmax(220px, 1fr))', gap: 12 }}>
          {products.map(product => (
            <div key={product.id} style={{ border: '1px solid #ddd', padding: 12, borderRadius: 8 }}>
              <h3>{product.name}</h3>
              <p>{product.description}</p>
              <p>
                <strong>${product.price}</strong>
              </p>
            </div>
          ))}
        </div>
      </section>
    </div>
  )
}
