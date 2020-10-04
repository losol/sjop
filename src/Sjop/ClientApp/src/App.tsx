import React from "react";
import Header from "./components/shared/Header";
import Footer from "./components/shared/Footer";
import ProductList from "./components/products/ProductList";

function App() {
  return (
    <>
      <Header />
      <main>
        <ProductList />
      </main>
      <Footer />
    </>
  );
}

export default App;
