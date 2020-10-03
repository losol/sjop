import React from "react";
import Header from "./Header";
import Footer from "./Footer";
import ProductList from "./ProductList";

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
