import React, {useEffect, useState} from 'react'
const API_BASE = import.meta.env.VITE_API_BASE_URL || 'http://localhost:5000/api'
export default function App(){
  const [products,setProducts]=useState([])
  useEffect(()=>{ fetch(`${API_BASE}/products`).then(r=>r.json()).then(setProducts).catch(()=>{}) },[])
  return <div style={{padding:20,fontFamily:'Arial'}}><h1>E‑commerce Demo</h1><div style={{display:'grid',gridTemplateColumns:'repeat(3,1fr)',gap:12}}>{products.map(p=> <div key={p.id} style={{border:'1px solid #ddd',padding:12,borderRadius:8}}><h3>{p.name}</h3><p>{p.description}</p><p><strong>${p.price}</strong></p></div>)}</div></div>
}
